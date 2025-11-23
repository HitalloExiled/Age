using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using Age.Services;
using Age.Shaders;
using ThirdParty.Vulkan.Enums;

namespace Age.Passes;

public class UISceneColorPass : UIScenePass
{
    private readonly ResourceCache<Texture2D, UniformSet> uniformSets = new();

    private UniformSet? lastUniformSet;

    [AllowNull]
    private CanvasStencilMaskShader canvasStencilWriterShader;

    [AllowNull]
    private CanvasStencilMaskShader canvasStencilEraserShader;

    [AllowNull]
    private CanvasShader shader;

    protected override CanvasStencilMaskShader CanvasStencilEraserShader => this.canvasStencilEraserShader;
    protected override CanvasStencilMaskShader CanvasStencilWriterShader => this.canvasStencilWriterShader;
    protected override Color                   ClearColor                => Color.Black;
    protected override CommandBuffer           CommandBuffer             => VulkanRenderer.Singleton.CurrentCommandBuffer;
    protected override CommandFilter           CommandFilter             => CommandFilter.Color;
    protected override CanvasShader            Shader                    => this.shader;

    public override string Name => nameof(UISceneColorPass);

    protected override void BeforeExecute() =>
        this.lastUniformSet = null;

    protected override void OnConnected()
    {
        base.OnConnected();

        Debug.Assert(this.RenderGraph != null);

        this.shader = new(this.Viewport!.RenderTarget, true);
        this.shader.Changed += RenderingService.Singleton.RequestDraw;

        this.canvasStencilWriterShader = new CanvasStencilMaskShader(this.RenderGraph.Viewport.RenderTarget, StencilOp.Write, true);
        this.canvasStencilWriterShader.Changed += RenderingService.Singleton.RequestDraw;

        this.canvasStencilEraserShader = new CanvasStencilMaskShader(this.RenderGraph.Viewport.RenderTarget, StencilOp.Erase, true);
        this.canvasStencilEraserShader.Changed += RenderingService.Singleton.RequestDraw;
    }

    protected override void OnDisconnecting()
    {
        base.OnDisconnecting();

        this.shader.Changed -= RenderingService.Singleton.RequestDraw;
        this.shader.Dispose();

        this.canvasStencilWriterShader.Changed -= RenderingService.Singleton.RequestDraw;
        this.canvasStencilWriterShader.Dispose();

        this.canvasStencilEraserShader.Changed -= RenderingService.Singleton.RequestDraw;
        this.canvasStencilEraserShader.Dispose();
    }

    protected override void Record(RectCommand command)
    {
        if (!this.uniformSets.TryGetValue(command.TextureMap.Texture, out var uniformSet))
        {
            var diffuse = new CombinedImageSamplerUniform
            {
                Binding     = 0,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                Image       = command.TextureMap.Texture.Image,
                ImageView   = command.TextureMap.Texture.ImageView,
                Sampler     = command.TextureMap.Texture.Sampler,
            };

            this.uniformSets.Set(command.TextureMap.Texture, uniformSet = new UniformSet(this.Shader, [diffuse]));
        }

        if (uniformSet != null  && this.lastUniformSet != uniformSet)
        {
            this.CommandBuffer.BindUniformSet(this.lastUniformSet = uniformSet);
        }

        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = command.Color,
            Flags     = command.Flags,
            Size      = command.Size,
            Transform = command.Matrix,
            UV        = command.TextureMap.UV,
            Viewport  = this.Viewport!.Size,
        };

        this.CommandBuffer.PushConstant(this.Shader, constant);
        this.CommandBuffer.DrawIndexed(this.IndexBuffer);
    }

    protected override void OnDisposed(bool disposing) =>
        this.OnDisconnecting();
}
