using Age.Commands;
using Age.Elements;
using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Scenes;
using Age.Shaders;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using System.Runtime.CompilerServices;
using Age.Services;

namespace Age.Passes;

public abstract class Scene2DPass : RenderPass<Texture2D>
{
    [InlineArray(2)]
    private struct ClearValuesDefault
    {
#pragma warning disable RCS1213
        private ClearValue _;
#pragma warning restore RCS1213

        public ClearValuesDefault()
        {
            this[0] = new(Color.Black);
            this[1] = new(1, 0);
        }
    }

    private readonly ClearValuesDefault clearValues = new();

    private readonly CanvasStencilMaskShader canvasStencilEraserShader;
    private readonly CanvasStencilMaskShader canvasStencilWriterShader;
    private readonly Stack<StencilLayer>     stencilStack = [];

    protected abstract Shader Shader { get; }

    protected IndexBuffer32                     IndexBuffer  { get; }
    protected VertexBuffer<CanvasShader.Vertex> VertexBuffer { get; }
    protected Viewport                          Viewport     { get; }

    protected override RenderTarget             RenderTarget => this.Viewport.RenderTarget;
    protected override ReadOnlySpan<ClearValue> ClearValues  => this.clearValues;

    [AllowNull]
    public override Texture2D Output => this.Viewport.Texture;

    protected Scene2DPass(Viewport viewport)
    {
        Span<CanvasShader.Vertex> vertices =
        [
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        ];

        this.VertexBuffer              = new(vertices);
        this.IndexBuffer               = new([0, 1, 2, 0, 2, 3]);
        this.canvasStencilWriterShader = new CanvasStencilMaskShader(viewport.RenderTarget, StencilOp.Write, true);
        this.canvasStencilEraserShader = new CanvasStencilMaskShader(viewport.RenderTarget, StencilOp.Erase, true);
        this.Viewport                  = viewport;
    }

    private void ClearStencilBuffer(in Size<uint> extent) =>
        this.ClearStencilBufferAttachment(extent, 0);

    private void ClearStencilBufferAttachment(in Size<uint> extent, uint value)
    {
        var attachment = new VkClearAttachment
        {
            AspectMask = VkImageAspectFlags.Stencil,
            ClearValue =
            {
                DepthStencil =
                {
                    Depth   = 1,
                    Stencil = value,
                }
            },
            ColorAttachment = 0,
        };

        var rect = new VkClearRect
        {
            Rect =
            {
                Extent =
                {
                    Width  = extent.Width,
                    Height = extent.Height
                }
            },
            BaseArrayLayer = 0,
            LayerCount     = 1,
        };

        this.CommandBuffer.ClearAttachments([attachment], [rect]);
    }

    private void EraseStencil(StencilLayer stencilLayer, in Size<uint> extent) =>
        this.SetStencil(this.canvasStencilEraserShader, stencilLayer, extent);

    private void SetStencil(Shader shader, StencilLayer stencilLayer, in Size<uint> viewportSize)
    {
        this.CommandBuffer.BindShader(shader);

        var constant = new CanvasShader.PushConstant
        {
            Size      = stencilLayer.Size,
            Transform = stencilLayer.Transform,
            Border    = stencilLayer.Border,
            Viewport  = viewportSize,
            UV        = UVRect.Normalized,
        };

        this.CommandBuffer.PushConstant(this.canvasStencilWriterShader, constant);
        this.CommandBuffer.DrawIndexed(this.IndexBuffer);
    }

    private void WriteStencil(StencilLayer stencilLayer, in Size<uint> extent) =>
        this.SetStencil(this.canvasStencilWriterShader, stencilLayer, extent);

    protected override void OnConnected()
    {
        base.OnConnected();

        if (this.Shader.Watching)
        {
            this.Shader.Changed += RenderingService.Singleton.RequestDraw;
        }

        if (this.canvasStencilEraserShader.Watching)
        {
            this.canvasStencilEraserShader.Changed += RenderingService.Singleton.RequestDraw;
        }

        if (this.canvasStencilWriterShader.Watching)
        {
            this.canvasStencilWriterShader.Changed += RenderingService.Singleton.RequestDraw;
        }
    }

    protected override void Record(RenderContext context)
    {
        this.Record(context.Buffer2D.Commands);
    }

    protected void Record<T>(ReadOnlySpan<T> commands) where T : Command
    {
        this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, 0u);

        this.CommandBuffer.BindShader(this.Shader);
        this.CommandBuffer.BindVertexBuffer(this.VertexBuffer);
        this.CommandBuffer.BindIndexBuffer(this.IndexBuffer);

        var previousDepth = 0u;
        var viewportSize  = this.Viewport.Size;

        for (var i = 0; i < commands.Length; i++)
        {
            var command = commands[i];

            if (!this.stencilStack.TryPeek(out var previousLayer) || command.StencilLayer != previousLayer)
            {
                if (command.StencilLayer is StencilLayer currentLayer)
                {
                    var currentDepth = currentLayer.Depth + 1;

                    while (this.stencilStack.Count > currentDepth)
                    {
                        var layer = this.stencilStack.Pop();

                        this.EraseStencil(layer, viewportSize);
                    }

                    if (currentDepth >= previousDepth)
                    {
                        this.WriteStencil(currentLayer, viewportSize);

                        if (currentDepth > previousDepth)
                        {
                            this.stencilStack.Push(currentLayer);
                        }
                    }

                    this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, currentDepth);

                    previousDepth = currentDepth;

                    this.CommandBuffer.BindShader(this.Shader);
                }
                else if (previousDepth > 0)
                {
                    this.stencilStack.Clear();
                    this.ClearStencilBuffer(viewportSize);
                    this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, previousDepth = 0u);
                    this.CommandBuffer.BindShader(this.Shader);
                }
            }

            switch (command)
            {
                case RectCommand rectCommand:
                    this.Record(rectCommand);

                    break;
                default:
                    throw new InvalidOperationException($"Unsupported command type '{command.GetType().FullName}'.");
            }
        }
    }

    protected virtual void Record(RectCommand command) { }
}
