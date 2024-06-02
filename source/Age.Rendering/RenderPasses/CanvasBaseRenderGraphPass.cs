using System.Runtime.CompilerServices;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.Rendering.RenderPasses;

public abstract class CanvasBaseRenderGraphPass(VulkanRenderer renderer, IWindow window) : RenderGraphPass(renderer, window)
{
    protected struct RenderResources(Shader shader, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
    {
        public Shader       Shader       = shader;
        public VertexBuffer VertexBuffer = vertexBuffer;
        public IndexBuffer  IndexBuffer  = indexBuffer;

        public readonly void Dispose()
        {
            this.Shader.Dispose();
            this.VertexBuffer.Dispose();
            this.IndexBuffer.Dispose();
        }
    }

    public event Action? Changed;

    protected abstract VkCommandBuffer   CommandBuffer  { get; }
    protected abstract RenderResources[] Resources { get; }
    protected abstract RenderPass        RenderPass     { get; }
    protected abstract uint              CurrentBuffer  { get; }

    protected abstract void ExecuteCommand(RenderResources resource, RectDrawCommand command, in Size<float> viewport, in Matrix3x2<float> transform);

    protected void NotifyChanged() =>
        this.Changed?.Invoke();

    protected virtual void AfterExecute() { }
    protected virtual void BeforeExecute() { }

    public override void Execute()
    {
        this.BeforeExecute();

        var viewport      = this.Window.ClientSize;
        var viewportFloat = viewport.Cast<float>();
        var extent        = Unsafe.As<Size<uint>, VkExtent2D>(ref viewport);

        this.Renderer.SetViewport(this.CommandBuffer, extent);
        this.Renderer.BeginRenderPass(this.CommandBuffer, this.RenderPass, this.CurrentBuffer, Color.White);

        foreach (var resource in this.Resources)
        {
            this.Renderer.BindPipeline(this.CommandBuffer,      resource.Shader);
            this.Renderer.BindVertexBuffers(this.CommandBuffer, resource.VertexBuffer);
            this.Renderer.BindIndexBuffer(this.CommandBuffer,   resource.IndexBuffer);

            foreach (var entry in this.Window.Tree.EnumerateCommands())
            {
                switch (entry.Command)
                {
                    case RectDrawCommand rectDrawCommand:
                        this.ExecuteCommand(resource, rectDrawCommand, viewportFloat, entry.Transform);
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
