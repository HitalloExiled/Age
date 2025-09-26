using Age.Numerics;

namespace Age.Rendering.Resources;

public partial class Texture
{
    public struct CreateInfo
    {
        public required Size<uint> Extent;

        public TextureAspect Aspect  = TextureAspect.Color;
        public TextureFormat Format  = TextureFormat.B8G8R8A8Unorm;
        public uint          Mipmap  = 1;
        public SampleCount   Samples = SampleCount.N1;
        public TextureType   Type    = TextureType.N2D;
        public TextureUsage  Usage   = TextureUsage.TransferSrc | TextureUsage.TransferDst | TextureUsage.Sampled;

        public CreateInfo() { }
    }
}
