using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Age.Cache;
using Age.Commands;
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
    private Geometry2DStencilMaskShader geometry2DStencilMaskWriterShader;

    [AllowNull]
    private Geometry2DStencilMaskShader geometry2DStencilMaskEraserShader;

    [AllowNull]
    private Geometry2DColorShader shader;

    protected override CommandBuffer               CommandBuffer                     => VulkanRenderer.Singleton.CurrentCommandBuffer;
    protected override CommandFilter               CommandFilter                     => CommandFilter.Color;
    protected override Geometry2DStencilMaskShader Geometry2DStencilMaskEraserShader => this.geometry2DStencilMaskEraserShader;
    protected override Geometry2DStencilMaskShader Geometry2DStencilMaskWriterShader => this.geometry2DStencilMaskWriterShader;
    protected override Geometry2DColorShader       Shader                            => this.shader;

    public override string Name => nameof(UISceneColorPass);

    protected override void BeforeExecute() =>
        this.lastUniformSet = null;

    protected override void OnConnected()
    {
        base.OnConnected();

        Debug.Assert(this.RenderGraph != null);

        this.shader = ShaderCache.Singleton.Get<Geometry2DColorShader>(this.Viewport!.RenderTarget);
        this.shader.Changed += RenderingService.Singleton.RequestDraw;

        // this.geometry2DStencilMaskWriterShader = new Geometry2DStencilMaskShader(this.RenderGraph.Viewport.RenderTarget.RenderPass, StencilOp.Write, true);
        this.geometry2DStencilMaskWriterShader = ShaderCache.Singleton.Get<Geometry2DStencilMaskShader>(this.Viewport!.RenderTarget, new() { StencilOp = StencilOp.Write });
        this.geometry2DStencilMaskWriterShader.Changed += RenderingService.Singleton.RequestDraw;

        // this.geometry2DStencilMaskEraserShader = new Geometry2DStencilMaskShader(this.RenderGraph.Viewport.RenderTarget.RenderPass, StencilOp.Erase, true);
        this.geometry2DStencilMaskEraserShader = ShaderCache.Singleton.Get<Geometry2DStencilMaskShader>(this.Viewport!.RenderTarget, new() { StencilOp = StencilOp.Erase });
        this.geometry2DStencilMaskEraserShader.Changed += RenderingService.Singleton.RequestDraw;
    }

    protected override void OnDisconnecting()
    {
        base.OnDisconnecting();

        this.shader.Changed -= RenderingService.Singleton.RequestDraw;
        this.shader.Dispose();

        this.geometry2DStencilMaskWriterShader.Changed -= RenderingService.Singleton.RequestDraw;
        this.geometry2DStencilMaskWriterShader.Dispose();

        this.geometry2DStencilMaskEraserShader.Changed -= RenderingService.Singleton.RequestDraw;
        this.geometry2DStencilMaskEraserShader.Dispose();
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

        var constant = new Geometry2DShader.PushConstant
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

    protected override void OnDisposed(bool disposing)
    {
        base.OnDisposed(disposing);
        this.OnDisconnecting();
    }
}
