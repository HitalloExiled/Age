using Age.Elements;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Rendering.Vulkan;
using Age.RenderPasses;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Scene;

public sealed partial class RenderTree : NodeTree
{
    #region 8-bytes
    private readonly List<Command2DEntry> command2DEntriesCache = [];
    private readonly List<Command3DEntry> command3DEntriesCache = [];

    private Buffer                     buffer = null!;
    private ulong                      bufferVersion;
    private CanvasIndexRenderGraphPass canvasIndexRenderGraphPass = null!;
    private Element?                   lastFocusedElement;
    private TextNode?                  lastFocusedTextNode;
    private Element?                   lastHoveredElement;
    private TextNode?                  lastHoveredTextNode;

    internal List<Node>    Nodes    { get; } = new(256);
    internal List<Scene3D> Scenes3D { get; } = [];

    public Window Window { get; }
    #endregion

    public RenderTree(Window window)
    {
        this.Window = window;

        this.Window.Closed += this.Dispose;
    }

    private IEnumerable<CommandEntry> EnumerateCommands()
    {
        var enumerator = this.Root.GetTraverseEnumerator();

        var index = 0;

        while (enumerator.MoveNext())
        {
            if (enumerator.Current is RenderNode current)
            {
                if (current.Visible == true)
                {
                    current.Index = index;

                    if (index == this.Nodes.Count)
                    {
                        this.Nodes.Add(current);
                    }
                    else
                    {
                        this.Nodes[index] = current;
                    }

                    index++;

                    if (current is Node2D node2D)
                    {
                        var transform = node2D.TransformCache;

                        foreach (var command in node2D.Commands)
                        {
                            var entry = new Command2DEntry(command, transform);

                            this.command2DEntriesCache.Add(entry);

                            yield return entry;
                        }
                    }
                    else if (current is Node3D node3D)
                    {
                        var transform = (Matrix4x4<float>)node3D.TransformCache;

                        foreach (var command in node3D.Commands)
                        {
                            var entry = new Command3DEntry(command, transform);

                            this.command3DEntriesCache.Add(entry);

                            yield return entry;
                        }
                    }
                }
                else
                {
                    enumerator.SkipToNextSibling();
                }
            }
        }

        if (index < this.Nodes.Count)
        {
            this.Nodes.RemoveRange(index, this.Nodes.Count - index);
        }
    }

    private Element? GetElement(ushort x, ushort y) =>
        this.GetNode(x, y, out _) switch
        {
            Element  element  => element,
            TextNode textNode => textNode.ParentElement,
            _ => null,
        };

    private unsafe Node? GetNode(ushort x, ushort y, out uint characterPosition)
    {
        var image = this.canvasIndexRenderGraphPass.ColorImage;

        if (x < image.Extent.Width && y < image.Extent.Height)
        {
            if (this.bufferVersion != Time.Redraws)
            {
                image.CopyToBuffer(this.buffer);
                this.bufferVersion = Time.Redraws;
            }

            this.buffer.Allocation.Memory.Map(0, (uint)this.buffer.Allocation.Size, 0, out var imageIndexBuffer);

            var imageIndex = new Span<uint>((uint*)imageIndexBuffer, (int)this.buffer.Allocation.Size / sizeof(uint));

            var index = x + y * image.Extent.Width;
            var pixel = imageIndex[(int)index];

            var id = (int)(pixel & 0x00000FFF) - 1;

            this.buffer.Allocation.Memory.Unmap();

            if (id > -1 && id < this.Nodes.Count)
            {
                characterPosition = ((pixel >> 12) & 0xFFF) - 1;

                return this.Nodes[id];
            }
        }

        characterPosition = 0;

        return null;
    }

    private void ResetCache()
    {
        this.command2DEntriesCache.Clear();
        this.command3DEntriesCache.Clear();
    }

    private void OnContext(in Platforms.Display.ContextEvent contextEvent)
    {
        var element = this.GetElement(contextEvent.X, contextEvent.Y);

        element?.InvokeContext(contextEvent);
    }

    private void OnDoubleClick(in Platforms.Display.MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var characterPosition);

        Element? element;

        if (node is TextNode textNode)
        {
            textNode.Layout.PropagateSelection(characterPosition);
            element = textNode.ParentElement;
        }
        else
        {
            element = node as Element;
        }

        element?.InvokeDoubleClick(mouseEvent);
    }

    private void OnKeyDown(Key key)
    {
        if (key == Key.C && Input.IsKeyPressed(Key.Control) && this.lastFocusedTextNode?.SelectedText is string selectedText)
        {
            this.Window.SetClipboardData(selectedText);
        }
    }

    private void OnMouseDown(in Platforms.Display.MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var characterPosition);

        Element? element;

        if (node is TextNode textNode)
        {
            element = textNode.ParentElement;

            if (mouseEvent.Button == mouseEvent.PrimaryButton)
            {
                if (mouseEvent.KeyStates.HasFlag(MouseKeyStates.Shift) && this.lastFocusedTextNode == textNode)
                {
                    textNode.Layout.UpdateSelection(mouseEvent.X, mouseEvent.Y, characterPosition);
                }
                else
                {
                    this.lastFocusedTextNode?.Layout.ClearSelection();

                    textNode.Layout.SetCaret(mouseEvent.X, mouseEvent.Y, characterPosition);
                }

                if (this.lastFocusedTextNode != textNode)
                {
                    this.lastFocusedTextNode?.Layout.ClearCaret();
                    this.lastFocusedTextNode = textNode;
                }
            }
        }
        else
        {
            element = node as Element;

            if (this.lastFocusedTextNode != null && this.lastFocusedTextNode.ParentElement != element)
            {
                if (mouseEvent.Button == mouseEvent.PrimaryButton)
                {
                    this.lastFocusedTextNode.Layout.ClearSelection();
                }

                this.lastFocusedTextNode.Layout.ClearCaret();
                this.lastFocusedTextNode = null;
            }
        }

        if (element != null)
        {
            if (mouseEvent.Button == mouseEvent.PrimaryButton)
            {
                element.InvokeActivate();
            }

            if (this.lastFocusedElement != element)
            {
                this.lastFocusedElement?.InvokeBlur(mouseEvent);
                this.lastFocusedElement = element;

            }

            if (!element.IsFocused)
            {
                element.InvokeFocus(mouseEvent);
            }
        }
        else
        {
            this.lastFocusedElement?.InvokeBlur(mouseEvent);
            this.lastFocusedElement  = null;
        }
    }

    private unsafe void OnMouseMove(in Platforms.Display.MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var character);

        var textNode = node as TextNode;
        var element  = textNode?.ParentElement ?? node as Element;

        if (element != null)
        {
            element.InvokeMouseMoved(mouseEvent);

            if (this.lastHoveredElement != element)
            {
                this.lastHoveredElement?.InvokeMouseOut(mouseEvent);
                this.lastHoveredElement = element;

                element.InvokeMouseOver(mouseEvent);
            }
        }
        else
        {
            this.lastHoveredElement?.InvokeMouseOut(mouseEvent);
            this.lastHoveredElement  = null;
        }

        var primaryButtonIsPressed =
            mouseEvent.PrimaryButton == MouseButton.Left && mouseEvent.KeyStates.HasFlag(MouseKeyStates.LeftButton)
            || mouseEvent.PrimaryButton == MouseButton.Right && mouseEvent.KeyStates.HasFlag(MouseKeyStates.RightButton);

        if (textNode != null)
        {
            if (primaryButtonIsPressed && textNode == this.lastFocusedTextNode)
            {
                textNode.Layout.UpdateSelection(mouseEvent.X, mouseEvent.Y, character);
            }

            if (this.lastHoveredTextNode != textNode)
            {
                this.lastHoveredTextNode?.Layout.TargetMouseOut();
                this.lastHoveredTextNode = textNode;

                textNode.Layout.TargetMouseOver();
            }
        }
        else if (this.lastHoveredTextNode != null)
        {
            if (primaryButtonIsPressed)
            {
                this.lastHoveredTextNode.Layout.UpdateSelection(mouseEvent.X, mouseEvent.Y);
            }
            else
            {
                this.lastHoveredTextNode?.Layout.TargetMouseOut();
                this.lastHoveredTextNode = null;
            }
        }
    }

    private void OnMouseUp(in Platforms.Display.MouseEvent mouseEvent)
    {
        var element = this.GetElement(mouseEvent.X, mouseEvent.Y);

        if (element != null)
        {
            if (mouseEvent.Button == mouseEvent.PrimaryButton)
            {
                element.InvokeDeactivate();
            }

            if (this.lastFocusedElement == element)
            {
                element.InvokeClick(mouseEvent);
            }

            this.lastFocusedElement = element;
        }
        else
        {
            this.lastFocusedElement?.InvokeDeactivate();
            this.lastFocusedElement = null;
        }
    }

    [MemberNotNull(nameof(buffer))]
    private unsafe void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(uint);

        this.buffer = VulkanRenderer.Singleton.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
    }

    internal IEnumerable<Command2DEntry> Enumerate2DCommands()
    {
        if (this.command2DEntriesCache.Count > 0)
        {
            for (var i = 0; i < this.command2DEntriesCache.Count; i++)
            {
                yield return this.command2DEntriesCache[i];
            }
        }
        else
        {
            foreach (var entry in this.EnumerateCommands())
            {
                if (entry.TryGetCommand2DEntry(out var command2DEntry))
                {
                    yield return command2DEntry;
                }
            }
        }
    }

    internal IEnumerable<Command3DEntry> Enumerate3DCommands()
    {
        if (this.command3DEntriesCache.Count > 0)
        {
            for (var i = 0; i < this.command3DEntriesCache.Count; i++)
            {
                yield return this.command3DEntriesCache[i];
            }
        }
        else
        {
            foreach (var entry in this.EnumerateCommands())
            {
                if (entry.TryGetCommand3DEntry(out var command3DEntry))
                {
                    yield return command3DEntry;
                }
            }
        }
    }

    protected override void Disposed(bool disposing)
    {
        if (disposing)
        {
            this.Window.Context     -= this.OnContext;
            this.Window.DoubleClick -= this.OnDoubleClick;
            this.Window.KeyDown     -= this.OnKeyDown;
            this.Window.MouseDown   -= this.OnMouseDown;
            this.Window.MouseMove   -= this.OnMouseMove;
            this.Window.MouseUp     -= this.OnMouseUp;

            this.Root.Dispose();
            this.buffer.Dispose();
        }
    }

    public sealed override void Initialize()
    {
        this.canvasIndexRenderGraphPass = RenderGraph.Active.GetRenderGraphPass<CanvasIndexRenderGraphPass>();
        this.canvasIndexRenderGraphPass.Recreated += this.UpdateBuffer;

        this.UpdateBuffer();

        this.Window.Context     += this.OnContext;
        this.Window.DoubleClick += this.OnDoubleClick;
        this.Window.KeyDown     += this.OnKeyDown;
        this.Window.MouseDown   += this.OnMouseDown;
        this.Window.MouseMove   += this.OnMouseMove;
        this.Window.MouseUp     += this.OnMouseUp;

        base.Initialize();
    }

    public sealed override void Update()
    {
        this.ResetCache();
        base.Update();
    }
}
