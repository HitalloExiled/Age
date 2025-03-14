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
    private Text?                      lastFocusedText;
    private Element?                   lastHoveredElement;
    private Text?                      lastHoveredText;

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
            if (enumerator.Current is Renderable current)
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

                    if (current is Spatial2D spatial2D)
                    {
                        var transform = spatial2D.TransformCache;

                        foreach (var command in spatial2D.Commands)
                        {
                            var entry = new Command2DEntry(command, transform);

                            this.command2DEntriesCache.Add(entry);

                            yield return entry;
                        }
                    }
                    else if (current is Spatial3D spatial3D)
                    {
                        var transform = (Matrix4x4<float>)spatial3D.TransformCache;

                        foreach (var command in spatial3D.Commands)
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
            Element element => element,
            Text    text    => text.ParentElement,
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

            this.buffer.Map(out var imageIndexBuffer);

            var imageIndex = new Span<ulong>(imageIndexBuffer.ToPointer(), (int)this.buffer.Size / sizeof(ulong));

            var index = x + y * image.Extent.Width;
            var pixel = imageIndex[(int)index];

            var id = (int)(pixel & 0x0000FFFFFF) - 1;

            this.buffer.Unmap();

            if (id > -1 && id < this.Nodes.Count)
            {
                characterPosition = (uint)((pixel >> 24) & 0xFFFFFF) - 1;

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

        if (node is Text text)
        {
            text.Layout.PropagateSelection(characterPosition);
            element = text.ParentElement;
        }
        else
        {
            element = node as Element;
        }

        element?.InvokeDoubleClick(mouseEvent, element != node);
    }

    private void OnKeyDown(Key key)
    {
        if (key == Key.C && Input.IsKeyPressed(Key.Control) && this.lastFocusedText?.CopySelected() is string selectedText)
        {
            this.Window.SetClipboardData(selectedText);
        }
    }

    private void OnMouseDown(in Platforms.Display.MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var characterPosition);

        Element? element;

        if (node is Text text)
        {
            element = text.ParentElement;

            if (mouseEvent.IsPrimaryButtonPressed)
            {
                text.InvokeActivate();

                if (mouseEvent.KeyStates.HasFlag(MouseKeyStates.Shift) && this.lastFocusedText == text)
                {
                    text.Layout.UpdateSelection(mouseEvent.X, mouseEvent.Y, characterPosition);
                }
                else
                {
                    this.lastFocusedText?.Layout.ClearSelection();

                    text.Layout.SetCaret(mouseEvent.X, mouseEvent.Y, characterPosition);
                }

                if (this.lastFocusedText != text)
                {
                    this.lastFocusedText?.Layout.ClearCaret();
                    this.lastFocusedText = text;
                }
            }
        }
        else
        {
            element = node as Element;

            if (this.lastFocusedText != null)
            {
                if (mouseEvent.Button == mouseEvent.PrimaryButton)
                {
                    this.lastFocusedText.Layout.ClearSelection();
                }

                this.lastFocusedText.Layout.ClearCaret();
            }

            this.lastFocusedText = null;
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

            element.InvokeMouseDown(mouseEvent, element != node);
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

        var text    = node as Text;
        var element = text?.ParentElement ?? node as Element;

        if (element != null)
        {
            if (this.lastHoveredElement != element)
            {
                this.lastHoveredElement?.InvokeMouseOut(mouseEvent);
                this.lastHoveredElement = element;

                element.InvokeMouseOver(mouseEvent);
            }

            element.InvokeMouseMoved(mouseEvent, element != node);
        }
        else
        {
            this.lastHoveredElement?.InvokeMouseOut(mouseEvent);
            this.lastHoveredElement  = null;
        }

        if (text != null)
        {
            if (mouseEvent.IsHoldingPrimaryButton && text == this.lastFocusedText)
            {
                text.Layout.UpdateSelection(mouseEvent.X, mouseEvent.Y, character);
            }

            if (this.lastHoveredText != text)
            {
                this.lastHoveredText?.Layout.TargetMouseOut();
                this.lastHoveredText = text;

                text.Layout.TargetMouseOver();
            }
        }
        else
        {
            this.lastHoveredText?.Layout.TargetMouseOut();
            this.lastHoveredText = null;
        }
    }

    private void OnMouseUp(in Platforms.Display.MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out _);

        var text    = node as Text;
        var element = text?.ParentElement ?? node as Element;

        if (element != null)
        {
            if (this.lastFocusedElement == element)
            {
                element.InvokeClick(mouseEvent, element != node);
            }

            element.InvokeMouseUp(mouseEvent, element != node);
        }

        this.lastFocusedElement?.InvokeDeactivate();
        this.lastFocusedText?.InvokeDeactivate();
    }

    [MemberNotNull(nameof(buffer))]
    private unsafe void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(ulong);

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
