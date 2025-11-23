using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Services;
using Age.Shaders;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Passes;

public sealed class UISceneEncodePass : UIScenePass
{

    [AllowNull]
    private CanvasStencilMaskShader canvasStencilWriterShader;

    [AllowNull]
    private CanvasStencilMaskShader canvasStencilEraserShader;

    [AllowNull]
    private RenderTarget renderTarget;

    [AllowNull]
    private CanvasShader shader;

    protected override CanvasStencilMaskShader CanvasStencilEraserShader => this.canvasStencilEraserShader;
    protected override CanvasStencilMaskShader CanvasStencilWriterShader => this.canvasStencilWriterShader;
    protected override Color                   ClearColor                => Color.Black;
    protected override CommandBuffer           CommandBuffer             { get; } = new(VkCommandBufferLevel.Primary);
    protected override CommandFilter           CommandFilter             => CommandFilter.Encode;
    protected override RenderTarget            RenderTarget              => this.renderTarget;
    protected override CanvasShader            Shader                    => this.shader;

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
                new()
                {
                    FinalLayout = ImageLayout.General,
                    SampleCount = SampleCount.N1,
                    Format      = TextureFormat.R16G16B16A16Unorm,
                    Usage       = TextureUsage.TransferDst | TextureUsage.TransferSrc | TextureUsage.Sampled | TextureUsage.ColorAttachment
                }
            ]
        };

        this.renderTarget = new(createInfo);
    }

    protected override void OnConnected()
    {
        Debug.Assert(this.Viewport != null);

        base.OnConnected();

        this.Viewport.Resized += this.CreateRenderTarget;

        this.CreateRenderTarget();

        this.shader = new CanvasEncodeShader(this.renderTarget, true);
        this.shader.Changed += RenderingService.Singleton.RequestDraw;

        this.canvasStencilWriterShader = new CanvasStencilMaskShader(this.renderTarget, StencilOp.Write, true);
        this.canvasStencilWriterShader.Changed += RenderingService.Singleton.RequestDraw;

        this.canvasStencilEraserShader = new CanvasStencilMaskShader(this.renderTarget, StencilOp.Erase, true);
        this.canvasStencilEraserShader.Changed += RenderingService.Singleton.RequestDraw;
    }

    protected override void OnDisconnecting()
    {
        Debug.Assert(this.Viewport != null);

        base.OnDisconnecting();

        this.Viewport.Resized -= this.CreateRenderTarget;

        this.shader.Changed -= RenderingService.Singleton.RequestDraw;
        this.shader.Dispose();

        this.canvasStencilWriterShader.Changed -= RenderingService.Singleton.RequestDraw;
        this.canvasStencilWriterShader.Dispose();

        this.canvasStencilEraserShader.Changed -= RenderingService.Singleton.RequestDraw;
        this.canvasStencilEraserShader.Dispose();

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

        var constant = new CanvasShader.PushConstant
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
        this.OnDisconnecting();

        this.CommandBuffer.Dispose();
    }
}
