using System.Runtime.CompilerServices;
using Age.Numerics;
using Age.Commands;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass(VulkanRenderer renderer, Window window) : RenderGraphPass(renderer, window)
{
    protected abstract Color             ClearColor    { get; }
    protected abstract CommandBuffer     CommandBuffer { get; }
    protected abstract Framebuffer       Framebuffer   { get; }
    protected abstract RenderResources[] Resources     { get; }

    protected abstract void ExecuteCommand(RenderResources resource, RectCommand command, in Size<float> viewport, in Matrix3x2<float> transform);

    protected virtual void AfterExecute() { }
    protected virtual void BeforeExecute() { }

    public override void Execute()
    {
        this.BeforeExecute();

        var viewport      = this.Window.ClientSize;
        var viewportFloat = viewport.Cast<float>();
        var extent        = Unsafe.As<Size<uint>, VkExtent2D>(ref viewport);

        this.CommandBuffer.SetViewport(extent);
        this.CommandBuffer.BeginRenderPass(this.RenderPass, this.Framebuffer, this.ClearColor);

        foreach (var resource in this.Resources)
        {
            if (resource.Enabled)
            {
                this.CommandBuffer.BindShader(resource.Shader);
                this.CommandBuffer.BindVertexBuffer(resource.VertexBuffer);
                this.CommandBuffer.BindIndexBuffer(resource.IndexBuffer);

                foreach (var entry in this.Window.Tree.Enumerate2DCommands())
                {
                    switch (entry.Command)
                    {
                        case RectCommand rectCommand:
                            this.ExecuteCommand(resource, rectCommand, viewportFloat, entry.Transform);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        this.CommandBuffer.EndRenderPass();

        this.AfterExecute();
    }
}
