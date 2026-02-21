using Age.Commands;
using Age.Rendering.Extensions;
using Age.Rendering.Resources;
using Age.Resources;
using Age.Scenes;
using Age.Services;
using Age.Shaders;
using Age.Storage;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Passes;

public class Scene3DEncodePass : Scene3DPass
{
    [AllowNull]
    private CommandBuffer commandBuffer;

    [AllowNull]
    private RenderTarget renderTarget;

    [AllowNull]
    private Geometry3DEncodeShader shader;

    public override string Name => nameof(Scene3DEncodePass);

    protected override CommandBuffer CommandBuffer => this.commandBuffer;
    protected override CommandFilter CommandFilter => CommandFilter.Encode;

    private void RecreateRenderTarget()
    {
        Debug.Assert(this.Viewport != null);

        this.renderTarget?.Dispose();
        this.renderTarget = RenderTargetFactory.ForCompositeEncode(this.Viewport.Size);
    }

    protected unsafe override void AfterExecute()
    {
        base.AfterExecute();

        if (this.Composite != null)
        {
            return;
        }

        this.CommandBuffer.End();

        var commandBufferHandle = this.CommandBuffer.Instance.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &commandBufferHandle
        };

        VulkanRenderer.Singleton.GraphicsQueue.Submit(submitInfo);
        VulkanRenderer.Singleton.GraphicsQueue.WaitIdle();
    }

    protected override void BeforeExecute()
    {
        base.BeforeExecute();

        if (this.Composite != null)
        {
            return;
        }

        this.CommandBuffer.Reset();
        this.CommandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);
    }

    protected override void OnConnected()
    {
        base.OnConnected();

        Debug.Assert(this.Viewport != null);

        if (this.Composite == null)
        {
            this.Viewport.Resized += this.RecreateRenderTarget;

            this.renderTarget  = RenderTargetFactory.ForEncode(this.Viewport.Size);
            this.commandBuffer = new(VkCommandBufferLevel.Primary);
        }
        else
        {
            this.renderTarget  = this.Composite.RenderTarget;
            this.commandBuffer = this.Composite.CommandBuffer;
        }

        this.shader = ShaderStorage.Singleton.Get<Geometry3DEncodeShader>(this.renderTarget, new() { Subpass = this.Index });
        this.shader.Changed += RenderingService.Singleton.RequestDraw;
    }

    protected override void OnDisconnecting()
    {
        base.OnDisconnecting();

        if (this.Composite == null)
        {
            this.Viewport?.Resized -= this.RecreateRenderTarget;

            this.renderTarget?.Dispose();
            this.commandBuffer?.Dispose();
        }

        if (this.shader != null)
        {
            this.shader.Changed -= RenderingService.Singleton.RequestDraw;
            this.shader.Dispose();
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        base.OnDisposed(disposing);
        this.OnDisconnecting();
    }

    protected override void Record(Camera3D camera, MeshCommand command)
    {
        var commandBuffer = this.CommandBuffer;

        var mesh       = Unsafe.As<Mesh>(command.Owner);
        var ubo        = this.UpdateUbo(camera, mesh, this.Viewport!.Size.ToExtent2D());
        var uniformSet = this.GetUniformSet(this.shader, mesh.Material, camera, ubo);

        var constant = new Geometry3DShader.PushConstant
        {
           Color = 0xFFFF_0000_0000_0000 | command.Metadata,
        };

        commandBuffer.BindShader(this.shader);
        commandBuffer.BindVertexBuffer([command.VertexBuffer]);
        commandBuffer.BindIndexBuffer(command.IndexBuffer);
        commandBuffer.BindUniformSet(uniformSet);
        commandBuffer.PushConstant(this.shader, constant);
        commandBuffer.DrawIndexed(command.IndexBuffer);
    }
}
