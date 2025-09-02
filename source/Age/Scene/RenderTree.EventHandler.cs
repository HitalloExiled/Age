using System.Runtime.CompilerServices;
using Age.Core.Extensions;
using Age.Elements;
using Age.Platforms.Display;

namespace Age.Scene;

internal delegate void WindowMouseEventHandler(in WindowMouseEvent windowMouseEvent, Node? node);

public sealed partial class RenderTree
{
    private readonly List<Layoutable> leftToAncestor  = [];
    private readonly List<Layoutable> rightToAncestor = [];

    internal event WindowMouseEventHandler? MouseDown;
    internal event WindowMouseEventHandler? MouseUp;
    internal event WindowMouseEventHandler? MouseMoved;

    private Element?      focusedElement;
    private Text?         focusedText;
    private Element?      hoveredElement;
    private Text?         hoveredText;
    private VirtualChild? hoveredVirtualChild;
    private Element?      pressedElement;
    private VirtualChild? pressedVirtualChild;

    private unsafe Node? GetNode(ushort x, ushort y, out uint virtualChildIndex)
    {
        var image = this.canvasIndexRenderGraphPass.ColorImage;

        if (x < image.Extent.Width && y < image.Extent.Height)
        {
            if (this.bufferVersion != Time.Redraws)
            {
                image.CopyToBuffer(this.buffer);
                this.bufferVersion = Time.Redraws;
            }

            this.buffer.Map(out var imageIndexBuffer);

            var imageIndex = new Span<ulong>(imageIndexBuffer.ToPointer(), (int)this.buffer.Size / sizeof(ulong));

            var index = x + (y * image.Extent.Width);
            var pixel = imageIndex[(int)index];

            var id = (int)(pixel & 0x0000FFFFFF) - 1;

            this.buffer.Unmap();

            if (id > -1 && id < this.Nodes.Count)
            {
                virtualChildIndex = (uint)((pixel >> 24) & 0xFFFFFF);

                return this.Nodes[id];
            }
        }

        virtualChildIndex = 0;

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DiscardDanglingHoveredNodes()
    {
        if (this.hoveredElement?.IsConnected == false)
        {
            this.hoveredElement = null;
        }

        if (this.hoveredText?.IsConnected == false)
        {
            this.hoveredText = null;
        }

        if (this.hoveredVirtualChild?.IsConnected == false)
        {
            this.hoveredText = null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DiscardDanglingFocusedNodes()
    {
        if (this.focusedElement?.IsConnected == false)
        {
            this.focusedElement = null;
        }

        if (this.focusedText?.IsConnected == false)
        {
            this.focusedText = null;
        }

        if (this.pressedVirtualChild?.IsConnected == false)
        {
            this.focusedText = null;
        }
    }

    private void OnContext(in WindowContextEvent contextEvent)
    {
        var node = this.GetNode(contextEvent.X, contextEvent.Y, out var virtualChildIndex);

        if (node is Element element)
        {
            element.InvokeContext(contextEvent);
        }
    }

    private void OnDoubleClick(in WindowMouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var virtualChildIndex);

        Element? element;

        if (node is Text text)
        {
            text.PropagateSelection(virtualChildIndex - 1);
            element = text.ComposedParentElement;
        }
        else
        {
            element = node as Element;
        }

        element?.InvokeDoubleClick(mouseEvent, element != node);
    }

    private void OnKeyDown(Key key)
    {
        if (key == Key.C && Input.IsKeyPressed(Key.Control) && this.focusedText?.CopySelected() is string selectedText)
        {
            this.Window.SetClipboardData(selectedText);
        }
    }

    private void OnMouseDown(in WindowMouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var virtualChildIndex);

        MouseDown?.Invoke(mouseEvent, node);

        this.DiscardDanglingFocusedNodes();

        Element? element;

        if (node is Text text)
        {
            element = text.ComposedParentElement;

            if (mouseEvent.IsPrimaryButtonPressed)
            {
                text.HandleActivate();

                if (mouseEvent.KeyStates.HasFlags(MouseKeyStates.Shift) && this.focusedText == text)
                {
                    text.HandleVirtualChildMouseDown(mouseEvent, virtualChildIndex, false);
                }
                else
                {
                    this.focusedText?.ClearSelection();

                    text.HandleVirtualChildMouseDown(mouseEvent, virtualChildIndex, true);
                }

                if (this.focusedText != text)
                {
                    this.focusedText?.ClearCaret();
                    this.focusedText = text;
                }
            }
        }
        else
        {
            element = node as Element;

            if (element == null || (element != this.focusedElement && !Element.IsScrollControl(virtualChildIndex)))
            {
                if (this.focusedText != null)
                {
                    if (mouseEvent.IsPrimaryButtonPressed)
                    {
                        this.focusedText.ClearSelection();
                    }

                    this.focusedText.ClearCaret();
                }

                this.focusedText = null;
            }
        }

        if (element != null)
        {
            if (element == node && virtualChildIndex != default)
            {
                var virtualChild = new VirtualChild(element, virtualChildIndex);

                this.pressedVirtualChild = virtualChild;

                virtualChild.HandleMouseDown(mouseEvent);
            }
            else
            {
                this.pressedVirtualChild = null;

                this.pressedElement = element;

                if (mouseEvent.IsPrimaryButtonPressed)
                {
                    element.InvokeActivate();
                }

                if (this.focusedElement != element)
                {
                    this.focusedElement?.InvokeBlur(mouseEvent);
                    this.focusedElement = element;
                }

                if (!element.IsFocused)
                {
                    element.InvokeFocus(mouseEvent);
                }

                element.InvokeMouseDown(mouseEvent, element != node);
            }
        }
        else
        {
            this.pressedVirtualChild = null;

            this.focusedElement?.InvokeBlur(mouseEvent);
            this.focusedElement = null;
        }
    }

    private void OnMouseMove(in WindowMouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var virtualChildIndex);

        MouseMoved?.Invoke(mouseEvent, node);

        this.DiscardDanglingHoveredNodes();

        var text    = node as Text;
        var element = text?.ComposedParentElement ?? node as Element;

        if (element != null)
        {
            if (element == node && virtualChildIndex != default)
            {
                var virtualChild = new VirtualChild(element, virtualChildIndex);

                if (virtualChild != this.hoveredVirtualChild)
                {
                    this.hoveredVirtualChild?.HandleMouseOut(mouseEvent);
                    this.hoveredVirtualChild = virtualChild;

                    virtualChild.HandleMouseOver(mouseEvent);
                }

                virtualChild.HandleMouseMoved(mouseEvent);
            }
            else
            {
                this.hoveredVirtualChild?.HandleMouseOut(mouseEvent);
                this.hoveredVirtualChild = null;

                if (this.hoveredElement != element)
                {
                    if (this.hoveredElement != null)
                    {
                        this.hoveredElement.InvokeMouseOut(mouseEvent);

                        Layoutable.GetComposedPathBetween(this.leftToAncestor, this.rightToAncestor, element, this.hoveredElement);

                        var limit = this.rightToAncestor.Count - 1;

                        for (var i = 0; i < limit; i++)
                        {
                            (this.rightToAncestor[i] as Element)?.InvokeMouseLeave(mouseEvent);
                        }
                    }
                    else
                    {
                        for (var current = element; current != null; current = current.ComposedParentElement)
                        {
                            this.leftToAncestor.Add(current);
                        }
                    }

                    for (var i = this.leftToAncestor.Count - 2; i > -1; i--)
                    {
                        (this.leftToAncestor[i] as Element)?.InvokeMouseEnter(mouseEvent);
                    }

                    element.InvokeMouseOver(mouseEvent);

                    this.hoveredElement = element;
                }

                element.InvokeMouseMoved(mouseEvent, element != node);
            }
        }
        else
        {
            this.hoveredVirtualChild?.HandleMouseOut(mouseEvent);

            if (this.hoveredElement != null)
            {
                this.hoveredElement.InvokeMouseOut(mouseEvent);

                for (var current = this.hoveredElement; current != null; current = current.ComposedParentElement)
                {
                    current.InvokeMouseLeave(mouseEvent);
                }
            }

            this.hoveredElement = null;
        }

        if (text != null)
        {
            if (this.hoveredText != text)
            {
                this.hoveredText?.HandleMouseOut();
                this.hoveredText = text;

                text.HandleMouseOver();
            }

            text.HandleVirtualChildMouseMove(mouseEvent, virtualChildIndex);
        }
        else
        {
            if (!Layoutable.IsSelectingText)
            {
                if (element == null)
                {
                    this.Window.Cursor = default;
                }
                else if (this.hoveredText?.ComposedParentElement == element)
                {
                    element.ApplyCursor();
                }
            }

            this.hoveredText?.HandleMouseOut();
            this.hoveredText = null;
        }
    }

    private void OnMouseUp(in WindowMouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var virtualChildIndex);

        MouseUp?.Invoke(mouseEvent, node);

        this.DiscardDanglingFocusedNodes();

        var text    = node as Text;
        var element = text?.ComposedParentElement ?? node as Element;

        var wasSelectingText = Layoutable.IsSelectingText;

        if (element != null)
        {
            var indirect = element != node;

            if (element == node && virtualChildIndex != default)
            {
                var virtualChild = new VirtualChild(element, virtualChildIndex);

                virtualChild.HandleMouseUp(mouseEvent);
            }
            else
            {
                if (this.pressedElement == element)
                {
                    element.InvokeClick(mouseEvent, indirect);
                }

                element.InvokeMouseUp(mouseEvent, indirect);
            }
        }

        this.pressedElement?.InvokeMouseRelease(mouseEvent, node != this.pressedElement);
        this.pressedElement = null;

        this.pressedVirtualChild?.HandleMouseRelease(mouseEvent);
        this.pressedVirtualChild = null;

        this.focusedElement?.InvokeDeactivate();
        this.focusedText?.HandleDeactivate();

        if (wasSelectingText != Layoutable.IsSelectingText)
        {
            this.hoveredElement      = null;
            this.hoveredText         = null;
            this.hoveredVirtualChild = null;

            this.OnMouseMove(mouseEvent);
        }
    }

    private void OnMouseWheel(in WindowMouseEvent mouseEvent)
    {
        var mouseLocalPosition = Input.GetMousePosition();

        var node = this.GetNode(mouseLocalPosition.X, mouseLocalPosition.Y, out var _);

        var text    = node as Text;
        var element = text?.ComposedParentElement ?? node as Element;

        element?.InvokeMouseWheel(mouseEvent);
    }
}
