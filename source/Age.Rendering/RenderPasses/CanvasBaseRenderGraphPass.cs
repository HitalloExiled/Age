using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.Rendering.RenderPasses;

public abstract class CanvasBaseRenderGraphPass : RenderGraphPass
{
    protected IndexBuffer  IndexBuffer { get; }
    protected VertexBuffer VertexBuffer { get; }

    protected abstract VkCommandBuffer CommandBuffer { get; }
    protected abstract Shader          Shader        { get; }
    protected abstract uint            CurrentBuffer { get; }

    protected CanvasBaseRenderGraphPass(VulkanRenderer renderer, IWindow window) : base(renderer, window)
    {
        this.IndexBuffer = renderer.CreateIndexBuffer([0u, 1, 2, 0, 2, 3]);

        var vertices = new CanvasShader.Vertex[4]
        {
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        };

        this.VertexBuffer = renderer.CreateVertexBuffer(vertices);
    }

    protected abstract void ExecuteCommand(RectDrawCommand command, in Size<float> viewport, in Matrix3x2<float> transform);

    protected virtual void AfterExecute() { }
    protected virtual void BeforeExecute() { }

    protected override void OnDispose()
    {
        this.IndexBuffer.Dispose();
        this.VertexBuffer.Dispose();
        base.OnDispose();
    }

    public override void Execute()
    {
        this.BeforeExecute();

        this.Renderer.SetViewport(this.CommandBuffer, this.Window.Surface);
        this.Renderer.BindIndexBuffer(this.CommandBuffer, this.IndexBuffer);
        this.Renderer.BindVertexBuffers(this.CommandBuffer, this.VertexBuffer);
        this.Renderer.BindPipeline(this.CommandBuffer, this.Shader);
        this.Renderer.BeginRenderPass(this.CommandBuffer, this.Shader, this.CurrentBuffer, Color.White);

        var viewport = this.Window.ClientSize.Cast<float>();

        foreach (var node in this.Window.Tree.Traverse<Node2D>(true))
        {
            var transform = (Matrix3x2<float>)node.TransformCache;

            foreach (var command in node.Commands)
            {
                switch (command)
                {
                    case RectDrawCommand rectDrawCommand:
                        this.ExecuteCommand(rectDrawCommand, viewport, transform);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        this.Renderer.EndRenderPass(this.CommandBuffer);

        this.AfterExecute();
    }
}
