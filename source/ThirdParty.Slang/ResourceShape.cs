namespace ThirdParty.Slang;

public enum ResourceShape
{
    ResourceBaseShapeMask      = 0x0F,
    ResourceNone               = 0x00,
    Texture1D                  = 0x01,
    Texture2D                  = 0x02,
    Texture3D                  = 0x03,
    TextureCube                = 0x04,
    TextureBuffer              = 0x05,
    StructuredBuffer           = 0x06,
    ByteAddressBuffer          = 0x07,
    ResourceUnknown            = 0x08,
    AccelerationStructure      = 0x09,
    TextureSubpass             = 0x0A,
    ResourceExtShapeMask       = 0x1F0,
    TextureFeedback            = 0x10,
    TextureShadow              = 0x20,
    TextureArray               = 0x40,
    TextureMultisample         = 0x80,
    TextureCombined            = 0x100,
    Texture1DArray             = Texture1D | TextureArray,
    Texture2DArray             = Texture2D | TextureArray,
    TextureCubeArray           = TextureCube | TextureArray,
    Texture2DMultisample       = Texture2D | TextureMultisample,
    Texture2DMultisampleArray  = Texture2D | TextureMultisample | TextureArray,
    TextureSubpassMultisample  = TextureSubpass | TextureMultisample,
}
