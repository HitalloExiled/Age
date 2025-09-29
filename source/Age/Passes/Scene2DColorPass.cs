using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using Age.Scenes;
using Age.Shaders;
using ThirdParty.Vulkan.Enums;

namespace Age.Passes;

public class Scene2DColorPass(Viewport viewport) : Scene2DPass(viewport)
{
    private readonly ResourceCache<Texture2D, UniformSet> uniformSets = new();

    private UniformSet? lastUniformSet;

    protected override CommandBuffer CommandBuffer => VulkanRenderer.Singleton.CurrentCommandBuffer;
    protected override Color         ClearColor    => Color.Black;

    protected override CanvasShader Shader { get; } = new(viewport.RenderTarget, true);

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

        if (uniformSet != null && this.lastUniformSet != uniformSet)
        {
            this.CommandBuffer.BindUniformSet(this.lastUniformSet = uniformSet);
        }

        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = command.Color,
            Flags     = command.Flags,
            Size      = command.Size,
            Transform = command.Transform,
            UV        = command.TextureMap.UV,
            Viewport  = this.Viewport.Size,
        };

        this.CommandBuffer.PushConstant(this.Shader, constant);
        this.CommandBuffer.DrawIndexed(this.IndexBuffer);
    }

    protected override void OnDisposed(bool disposing)
    {
        this.Shader.Dispose();
    }
}
