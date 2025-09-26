using System.Diagnostics.CodeAnalysis;
using Age.Commands;
using Age.Elements;
using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Shaders;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Passes;

public abstract class Scene2DPass : RenderPass<Texture2D>
{
    private readonly Stack<StencilLayer>               stencilStack = [];
    private readonly IndexBuffer32                     indexBuffer;
    private readonly VertexBuffer<CanvasShader.Vertex> vertexBuffer;
    private readonly Shader                            canvasStencilEraserShader;
    private readonly Shader                            canvasStencilWriterShader;

    protected abstract Shader Shader { get; }

    [AllowNull]
    public override Texture2D Output
    {
        get;
        set => field = value ?? Texture2D.Empty;
    } = Texture2D.Empty;

    protected Scene2DPass()
    {
        var vertices = new CanvasShader.Vertex[4]
        {
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        };

        this.vertexBuffer  = new(vertices.AsSpan());
        this.indexBuffer   = new([0, 1, 2, 0, 2, 3]);

        this.canvasStencilWriterShader = new CanvasStencilMaskShader(this.RenderTarget, StencilOp.Write, true);
        this.canvasStencilEraserShader = new CanvasStencilMaskShader(this.RenderTarget, StencilOp.Erase, true);
    }

    private void ClearStencilBuffer(in VkExtent2D extent) =>
        this.ClearStencilBufferAttachment(extent, 0);

    private unsafe void ClearStencilBufferAttachment(in VkExtent2D extent, uint value)
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
            Rect           = new() { Extent = extent },
            BaseArrayLayer = 0,
            LayerCount     = 1,
        };

        this.CommandBuffer.ClearAttachments([attachment], [rect]);
    }

    private unsafe void EraseStencil(StencilLayer stencilLayer, in Size<float> viewport) =>
        this.SetStencil(this.canvasStencilEraserShader, stencilLayer, viewport);

    private void FillStencilBuffer(in VkExtent2D extent) =>
        this.ClearStencilBufferAttachment(extent, 1);

    private unsafe void SetStencil(Shader shader, StencilLayer stencilLayer, in Size<float> viewport)
    {
        this.CommandBuffer.BindShader(shader);

        var constant = new CanvasShader.PushConstant
        {
            Size      = stencilLayer.Size.Cast<float>(),
            Transform = stencilLayer.Transform,
            Border    = stencilLayer.Border,
            Viewport  = viewport,
            UV        = UVRect.Normalized,
        };

        this.CommandBuffer.PushConstant(this.canvasStencilWriterShader, constant);
        this.CommandBuffer.DrawIndexed(this.indexBuffer);
    }

    private unsafe void WriteStencil(StencilLayer stencilLayer, in Size<float> viewport) =>
        this.SetStencil(this.canvasStencilWriterShader, stencilLayer, viewport);

    protected override RenderTarget RenderTarget => this.RenderGraph?.Viewport.RenderTarget ?? throw new InvalidOperationException("Pass or pipeline no connected to any render graph");

    protected abstract void Record(ReadOnlySpan<Command2D> commands);

    protected override void Record(RenderContext context)
    {
        // this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, 0u);

        // this.CommandBuffer.BindShader(this.Shader);
        // this.CommandBuffer.BindVertexBuffer(this.vertexBuffer);
        // this.CommandBuffer.BindIndexBuffer(this.indexBuffer);

        // var previousDepth = 0u;

        // var commands = context.Commands2D;

        // for (var i = 0; i < commands.Length; i++)
        // {
        //     var command = commands[i];

            // if (this.PipelineVariants.HasAnyFlag(command.PipelineVariant))
            // {
            //     if (!pipeline.IgnoreStencil && (!this.stencilStack.TryPeek(out var previousLayer) || command.StencilLayer != previousLayer))
            //     {
            //         if (command.StencilLayer is StencilLayer currentLayer)
            //         {
            //             var currentDepth = currentLayer.Depth + 1;

            //             while (this.stencilStack.Count > currentDepth)
            //             {
            //                 var layer = this.stencilStack.Pop();

            //                 this.EraseStencil(layer, viewport);
            //             }

            //             if (currentDepth >= previousDepth)
            //             {
            //                 this.WriteStencil(currentLayer, viewport);

            //                 if (currentDepth > previousDepth)
            //                 {
            //                     this.stencilStack.Push(currentLayer);
            //                 }
            //             }

            //             this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, currentDepth);

            //             previousDepth = currentDepth;

            //             this.CommandBuffer.BindShader(this.Shader);
            //         }
            //         else if (previousDepth > 0)
            //         {
            //             this.stencilStack.Clear();
            //             this.ClearStencilBuffer(extent);
            //             this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, previousDepth = 0u);
            //             this.CommandBuffer.BindShader(pipeline.Shader);
            //         }
            //     }

            //     switch (command)
            //     {
            //         case RectCommand rectCommand:
            //             this.ExecuteCommand(pipeline, rectCommand, viewport);

            //             break;
            //         default:
            //             throw new InvalidOperationException($"Unsupported command type '{command.GetType().FullName}'.");
            //     }
            // }
        // }
    }
}
