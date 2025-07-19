using Age.Core.Extensions;
using Age.Elements;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Rendering.Vulkan;
using Age.RenderPasses;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Scene;

public sealed partial class RenderTree : NodeTree
{
    private readonly List<Command2DEntry> command2DEntries  = [];
    private readonly List<Command3DEntry> command3DEntries  = [];
    private readonly Stack<(Slot, int)>   composedTreeStack = [];

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

    public RenderTree(Window window)
    {
        this.Window = window;

        this.Window.Closed += this.Dispose;
    }

    private void BuildIndexAndCollectCommands()
    {
        var index = 0;

        var traversalEnumerator = this.Root.GetTraversalEnumerator();

        while (traversalEnumerator.MoveNext())
        {
            if (traversalEnumerator.Current is Renderable renderable && renderable.Visible)
            {
                updateIndex(renderable);

                if (renderable is Canvas canvas)
                {
                    traversalEnumerator.SkipToNextSibling();

                    var composedTreeTraversalEnumerator = canvas.GetComposedTreeTraversalEnumerator(this.composedTreeStack);

                    while (composedTreeTraversalEnumerator.MoveNext())
                    {
                        if (composedTreeTraversalEnumerator.Current.Visible)
                        {
                            updateIndex(composedTreeTraversalEnumerator.Current);

                            collect2D(composedTreeTraversalEnumerator.Current);
                        }
                        else
                        {
                            composedTreeTraversalEnumerator.SkipToNextSibling();
                        }
                    }
                }
                else if (renderable is Spatial2D spatial2D)
                {
                    collect2D(spatial2D);
                }
                else if (renderable is Spatial3D spatial3D)
                {
                    collect3D(spatial3D);
                }
            }
        }

        if (index < this.Nodes.Count)
        {
            this.Nodes.RemoveRange(index, this.Nodes.Count - index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collect2D(Spatial2D spatial2D)
        {
            var transform = spatial2D.TransformCache;

            foreach (var command in spatial2D.Commands)
            {
                var entry = new Command2DEntry(command, transform);

                this.command2DEntries.Add(entry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collect3D(Spatial3D spatial3D)
        {
            var transform = (Matrix4x4<float>)spatial3D.TransformCache;

            foreach (var command in spatial3D.Commands)
            {
                var entry = new Command3DEntry(command, transform);

                this.command3DEntries.Add(entry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void updateIndex(Node current)
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
        }
    }

    private Element? GetElement(ushort x, ushort y) =>
        this.GetNode(x, y, out _) switch
        {
            Element element => element,
            Text    text    => text.ComposedParentElement,
            _               => null,
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

            var index = x + (y * image.Extent.Width);
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

    private void OnContext(in ContextEvent contextEvent)
    {
        var element = this.GetElement(contextEvent.X, contextEvent.Y);

        element?.InvokeContext(contextEvent);
    }

    private void OnDoubleClick(in MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var characterPosition);

        Element? element;

        if (node is Text text)
        {
            text.PropagateSelection(characterPosition);
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
        if (key == Key.C && Input.IsKeyPressed(Key.Control) && this.lastFocusedText?.CopySelected() is string selectedText)
        {
            this.Window.SetClipboardData(selectedText);
        }
    }

    private void OnMouseDown(in MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var characterPosition);

        Element? element;

        if (node is Text text)
        {
            element = text.ComposedParentElement;

            if (mouseEvent.IsPrimaryButtonPressed)
            {
                text.InvokeActivate();

                if (mouseEvent.KeyStates.HasFlags(MouseKeyStates.Shift) && this.lastFocusedText == text)
                {
                    text.UpdateSelection(mouseEvent.X, mouseEvent.Y, characterPosition);
                }
                else
                {
                    this.lastFocusedText?.ClearSelection();

                    text.SetCaret(mouseEvent.X, mouseEvent.Y, characterPosition);
                }

                if (this.lastFocusedText != text)
                {
                    this.lastFocusedText?.ClearCaret();
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
                    this.lastFocusedText.ClearSelection();
                }

                this.lastFocusedText.ClearCaret();
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
            this.lastFocusedElement = null;
        }
    }

    private unsafe void OnMouseMove(in MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out var character);

        var text    = node as Text;
        var element = text?.ComposedParentElement ?? node as Element;

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
            this.lastHoveredElement = null;
        }

        if (text != null)
        {
            if (mouseEvent.IsHoldingPrimaryButton && text == this.lastFocusedText)
            {
                text.UpdateSelection(mouseEvent.X, mouseEvent.Y, character);
            }

            if (this.lastHoveredText != text)
            {
                this.lastHoveredText?.HandleMouseOut();
                this.lastHoveredText = text;

                text.HandleMouseOver();
            }
        }
        else
        {
            this.lastHoveredText?.HandleMouseOut();
            this.lastHoveredText = null;
        }

        if (!Layoutable.IsSelectingText && element == null && text == null)
        {
            this.Window.Cursor = default;
        }
    }

    private void OnMouseUp(in MouseEvent mouseEvent)
    {
        var node = this.GetNode(mouseEvent.X, mouseEvent.Y, out _);

        var text    = node as Text;
        var element = text?.ComposedParentElement ?? node as Element;

        var wasSelectingText = Layoutable.IsSelectingText;

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

        if (wasSelectingText != Layoutable.IsSelectingText)
        {
            this.lastHoveredElement = null;
            this.lastHoveredText = null;

            this.OnMouseMove(mouseEvent);
        }
    }

    private void ResetCache()
    {
        this.command2DEntries.Clear();
        this.command3DEntries.Clear();
    }

    [MemberNotNull(nameof(buffer))]
    private unsafe void UpdateBuffer()
    {
        this.buffer?.Dispose();

        var image = this.canvasIndexRenderGraphPass.ColorImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(ulong);

        this.buffer = VulkanRenderer.Singleton.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
    }

    protected override void OnDisposed(bool disposing)
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

    internal IEnumerable<Command2DEntry> Get2DCommands() =>
        this.command2DEntries;

    internal IEnumerable<Command3DEntry> Get3DCommands() =>
        this.command3DEntries;

    public override void Initialize()
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

    public override void Update()
    {
        base.Update();

        if (this.IsDirty)
        {
            this.ResetCache();
            this.BuildIndexAndCollectCommands();
        }
    }
}
