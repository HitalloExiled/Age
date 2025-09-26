using Age.Numerics;
using Age.Rendering.Extensions;

namespace Age.Rendering.Resources;

public partial class Texture2D : Texture
{
    public Size<uint> Size => this.Extent.ToSize();

    public Texture2D(in CreateInfo createInfo) : base(createInfo)
    { }

    public Texture2D(in CreateInfo createInfo, in Color clearColor) : base(createInfo, clearColor)
    { }

    public Texture2D(in CreateInfo createInfo, ReadOnlySpan<byte> buffer) : base(createInfo, buffer)
    { }

    internal Texture2D(Image image, bool owner, TextureAspect aspect) : base(image, owner, aspect)
    { }
}
