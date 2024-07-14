using Age.Numerics;
using Age.Rendering.Extensions;

namespace Age.Rendering.Resources;

public class RenderTargetOld(Texture texture, Framebuffer framebuffer) : Resource
{
    public Texture     Texture     { get; } = texture;
    public Framebuffer Framebuffer { get; } = framebuffer;

    public Size<uint> Size => this.Framebuffer.Extent.ToSize();

    protected override void OnDispose()
    {
        this.Framebuffer.Dispose();
        this.Texture.Dispose();
    }
}
