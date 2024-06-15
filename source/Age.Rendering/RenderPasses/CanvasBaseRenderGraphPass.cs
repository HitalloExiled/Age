using System.Runtime.CompilerServices;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.Rendering.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass(VulkanRenderer renderer, IWindow window) : RenderGraphPass(renderer, window)
{
    public event Action? Changed;

    protected abstract CommandBuffer     CommandBuffer { get; }
    protected abstract RenderResources[] Resources     { get; }
    protected abstract Framebuffer       Framebuffer   { get; }

    protected abstract void ExecuteCommand(RenderResources resource, RectCommand command, in Size<float> viewport, in Matrix3x2<float> transform);

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

        this.CommandBuffer.SetViewport(extent);
        this.CommandBuffer.BeginRenderPass(this.RenderPass, this.Framebuffer, Color.White);

        foreach (var resource in this.Resources)
        {
            if (resource.Enabled)
            {
                this.CommandBuffer.BindPipeline(resource.Shader);
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
