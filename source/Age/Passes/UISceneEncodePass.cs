using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Services;
using Age.Shaders;
using Age.Storage;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

namespace Age.Passes;

public sealed class UISceneEncodePass : UIScenePass
{
    [AllowNull]
    private CommandBuffer commandBuffer;

    [AllowNull]
    private Geometry2DStencilMaskShader geometry2DStencilMaskWriterShader;

    [AllowNull]
    private Geometry2DStencilMaskShader geometry2DStencilMaskEraserShader;

    [AllowNull]
    private RenderTarget renderTarget;

    [AllowNull]
    private Geometry2DEncodeShader shader;

    protected override CommandBuffer               CommandBuffer                     => this.commandBuffer;
    protected override CommandFilter               CommandFilter                     => CommandFilter.Encode;
    protected override Geometry2DStencilMaskShader Geometry2DStencilMaskEraserShader => this.geometry2DStencilMaskEraserShader;
    protected override Geometry2DStencilMaskShader Geometry2DStencilMaskWriterShader => this.geometry2DStencilMaskWriterShader;
    protected override RenderTarget                RenderTarget                      => this.renderTarget;
    protected override Geometry2DEncodeShader      Shader                            => this.shader;

    public override Texture2D Output => this.renderTarget?.ColorAttachments[0].Texture ?? Texture2D.Default;
    public override string    Name   => nameof(UISceneEncodePass);

    [MemberNotNull(nameof(renderTarget))]
    private void CreateRenderTarget()
    {
        Debug.Assert(this.Viewport != null);

        var createInfo = new RenderTarget.CreateInfo
        {
            Size             = this.Viewport.Size,
            ColorAttachments =
            [
                new RenderTarget.CreateInfo.ColorAttachmentInfo
                {
                    FinalLayout = ImageLayout.General,
                    SampleCount = SampleCount.N1,
                    Format      = TextureFormat.R16G16B16A16Unorm,
                    Usage       = TextureUsage.TransferDst | TextureUsage.TransferSrc | TextureUsage.Sampled | TextureUsage.ColorAttachment
                }
            ],
            DepthStencilAttachment = new()
            {
                FinalLayout = ImageLayout.DepthStencilAttachmentOptimal,
                Format      = VulkanRenderer.Singleton.StencilBufferFormat,
                Aspect      = TextureAspect.Stencil,
            }
        };

        this.renderTarget = new(createInfo);
    }

    protected override void OnConnected()
    {
        Debug.Assert(this.Viewport != null);

        base.OnConnected();

        this.Viewport.Resized += this.CreateRenderTarget;

        this.CreateRenderTarget();

        this.commandBuffer = new(VkCommandBufferLevel.Primary);

        this.shader = ShaderStorage.Singleton.Get<Geometry2DEncodeShader>(this.renderTarget, new() { Subpass = this.Index });
        this.shader.Changed += RenderingService.Singleton.RequestDraw;

        this.geometry2DStencilMaskWriterShader = ShaderStorage.Singleton.Get<Geometry2DStencilMaskShader>(this.renderTarget, new() { StencilOp = StencilOp.Write, Subpass = this.Index });
        this.geometry2DStencilMaskWriterShader.Changed += RenderingService.Singleton.RequestDraw;

        this.geometry2DStencilMaskEraserShader = ShaderStorage.Singleton.Get<Geometry2DStencilMaskShader>(this.renderTarget, new() { StencilOp = StencilOp.Erase, Subpass = this.Index });
        this.geometry2DStencilMaskEraserShader.Changed += RenderingService.Singleton.RequestDraw;
    }

    protected override void OnDisconnecting()
    {
        Debug.Assert(this.Viewport != null);

        base.OnDisconnecting();

        this.Viewport.Resized -= this.CreateRenderTarget;

        this.commandBuffer.Dispose();

        this.shader.Changed -= RenderingService.Singleton.RequestDraw;
        this.shader.Dispose();

        this.geometry2DStencilMaskWriterShader.Changed -= RenderingService.Singleton.RequestDraw;
        this.geometry2DStencilMaskWriterShader.Dispose();

        this.geometry2DStencilMaskEraserShader.Changed -= RenderingService.Singleton.RequestDraw;
        this.geometry2DStencilMaskEraserShader.Dispose();

        this.renderTarget.Dispose();
    }

    protected unsafe override void AfterExecute()
    {
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

        this.CommandBuffer.Reset();
        this.CommandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);
    }

    protected override void Record(RectCommand command)
    {
        Debug.Assert(this.Viewport != null);

        var constant = new Geometry2DShader.PushConstant
        {
            Border    = command.Border,
            Color     = 0xFFFF_0000_0000_0000 | command.Metadata,
            Flags     = command.Flags,
            Size      = command.Size,
            Transform = command.Matrix,
            UV        = UVRect.Normalized,
            Viewport  = this.Viewport.Size,
        };

        this.CommandBuffer.PushConstant(this.Shader, constant);
        this.CommandBuffer.DrawIndexed(this.IndexBuffer);
    }

    protected override void OnDisposed(bool disposing)
    {
        base.OnDisposed(disposing);
        this.OnDisconnecting();
    }
}
