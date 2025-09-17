using Age.Numerics;

namespace Age.Resources;

public partial class Texture2D
{
    public new struct CreateInfo
    {
        public required Size<uint> Size;

        public TextureAspect Aspect  = TextureAspect.Color;
        public TextureFormat Format  = TextureFormat.B8G8R8A8Unorm;
        public uint          Mipmap  = 1;
        public SampleCount   Samples = SampleCount.N1;
        public TextureUsage  Usage   = TextureUsage.TransferSrc | TextureUsage.TransferDst | TextureUsage.Sampled;

        public CreateInfo() { }

        public static implicit operator Texture.CreateInfo(in CreateInfo createInfo) =>
            new()
            {
                Extent  = createInfo.Size,
                Aspect  = createInfo.Aspect,
                Format  = createInfo.Format,
                Mipmap  = createInfo.Mipmap,
                Samples = createInfo.Samples,
                Usage   = createInfo.Usage,
                Type    = TextureType.N2D,
            };
    }
}
