using Age.Platforms.Display;

namespace Age.Elements;

public abstract partial class Element
{
    internal void HandleVirtualChildMouseDown(in WindowMouseEvent windowMouseEvent, uint virtualChildIndex)
    {
        if (this.CanScroll)
        {
            this.HandleScrollMouseDown(windowMouseEvent, (LayoutCommand)virtualChildIndex);
        }
    }

    internal void HandleVirtualChildMouseMoved(in WindowMouseEvent _1, uint _2)
    {
        if (this.CanScroll)
        {
            this.HandleScrollMouseMoved();
        }
    }

    internal void HandleVirtualChildMouseOut(in WindowMouseEvent _, uint virtualChildIndex)
    {
        if (this.CanScroll)
        {
            this.HandleScrollMouseOut((LayoutCommand)virtualChildIndex);
        }
    }

    internal void HandleVirtualChildMouseOver(in WindowMouseEvent _, uint virtualChildIndex)
    {
        if (this.CanScroll)
        {
            this.HandleScrollMouseOver((LayoutCommand)virtualChildIndex);
        }
    }

    internal void HandleVirtualChildMouseRelease(in WindowMouseEvent windowMouseEvent, uint virtualChildIndex)
    {
        if (this.CanScroll)
        {
            this.HandleScrollMouseRelease(windowMouseEvent, (LayoutCommand)virtualChildIndex);
        }
    }

    internal void HandleVirtualChildMouseUp(in WindowMouseEvent _1, uint _2)
    {
        // Just for symmetry
    }
}
