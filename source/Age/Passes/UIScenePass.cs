using Age.Core.Extensions;
using Age.Commands;
using Age.Elements;
using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Shaders;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

namespace Age.Passes;

public abstract partial class UIScenePass : RenderPass<Texture2D>
{
    private readonly ClearValuesDefault  clearValues  = new();
    private readonly Stack<StencilLayer> stencilStack = [];

    protected abstract CanvasStencilMaskShader CanvasStencilEraserShader { get; }
    protected abstract CanvasStencilMaskShader CanvasStencilWriterShader { get; }
    protected abstract CommandFilter           CommandFilter             { get; }

    protected abstract Shader Shader { get; }

    protected IndexBuffer32                     IndexBuffer    { get; }
    protected VertexBuffer<CanvasShader.Vertex> VertexBuffer   { get; }

    protected override RenderTarget             RenderTarget => this.RenderGraph!.Viewport.RenderTarget;
    protected override ReadOnlySpan<ClearValue> ClearValues  => this.clearValues;

    [AllowNull]
    public override Texture2D Output => this.RenderGraph?.Viewport.Texture ?? Texture2D.Default;

    protected UIScenePass()
    {
        Span<CanvasShader.Vertex> vertices =
        [
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        ];

        this.VertexBuffer = new(vertices);
        this.IndexBuffer  = new([0, 1, 2, 0, 2, 3]);
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
        this.SetStencil(this.CanvasStencilEraserShader, stencilLayer, extent);

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

        this.CommandBuffer.PushConstant(this.CanvasStencilWriterShader, constant);
        this.CommandBuffer.DrawIndexed(this.IndexBuffer);
    }

    private void WriteStencil(StencilLayer stencilLayer, in Size<uint> extent) =>
        this.SetStencil(this.CanvasStencilWriterShader, stencilLayer, extent);

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.VertexBuffer.Dispose();
            this.IndexBuffer.Dispose();
        }
    }

    protected override void Record(RenderContext context) =>
        this.Record(context.UIBuffer.Commands);

    protected void Record<T>(ReadOnlySpan<T> commands) where T : Command
    {
        var viewport = this.Viewport!.Size;

        this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, 0u);

        this.CommandBuffer.BindShader(this.Shader);
        this.CommandBuffer.BindVertexBuffer(this.VertexBuffer);
        this.CommandBuffer.BindIndexBuffer(this.IndexBuffer);

        var previousDepth = 0u;

        for (var i = 0; i < commands.Length; i++)
        {
            var command = commands[i];

            if (this.CommandFilter.HasAnyFlag(command.CommandFilter))
            {
                if (!this.stencilStack.TryPeek(out var previousLayer) || command.StencilLayer != previousLayer)
                {
                    if (command.StencilLayer is StencilLayer currentLayer)
                    {
                        var currentDepth = (uint)(currentLayer.Depth + 1);

                        while (this.stencilStack.Count > currentDepth)
                        {
                            var layer = this.stencilStack.Pop();

                            this.EraseStencil(layer, viewport);
                        }

                        if (currentDepth >= previousDepth)
                        {
                            this.WriteStencil(currentLayer, viewport);

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
                        this.ClearStencilBuffer(viewport);
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

        this.stencilStack.Clear();
    }

    protected virtual void Record(RectCommand command) { }
}
