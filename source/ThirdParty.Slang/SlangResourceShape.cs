namespace ThirdParty.Slang;

public enum SlangResourceShape
{
    BaseShapeMask             = 0x0F,
    None                      = 0x00,
    Texture1D                 = 0x01,
    Texture2D                 = 0x02,
    Texture3D                 = 0x03,
    TextureCube               = 0x04,
    TextureBuffer             = 0x05,
    StructuredBuffer          = 0x06,
    ByteAddressBuffer         = 0x07,
    Unknown                   = 0x08,
    AccelerationStructure     = 0x09,
    TextureSubpass            = 0x0A,
    ExtShapeMask              = 0xF0,
    TextureFeedbackFlag       = 0x10,
    TextureShadowFlag         = 0x20,
    TextureArrayFlag          = 0x40,
    TextureMultisampleFlag    = 0x80,
    Texture1DArray            = Texture1D | TextureArrayFlag,
    Texture2DArray            = Texture2D | TextureArrayFlag,
    TextureCubeArray          = TextureCube | TextureArrayFlag,
    Texture2DMultisample      = Texture2D | TextureMultisampleFlag,
    Texture2DMultisampleArray = Texture2D | TextureMultisampleFlag | TextureArrayFlag,
    TextureSubpassMultisample = TextureSubpass | TextureMultisampleFlag,
}
