using Age.Core.IO;
using Age.Core.Math;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;

namespace Age.Drivers.GLES3;

internal record RenderTarget
{
    public uint           Backbuffer          { get; set; }
    public uint           BackbufferFbo       { get; set; }
    public Color          ClearColor          { get; set; }
    public bool           ClearRequest        { get; set; }
    public uint           Color               { get; set; }
    public GLEnum         ColorFormat         { get; set; }
    public GLEnum         ColorInternalFormat { get; set; }
    public GLEnum         ColorType           { get; set; }
    public uint           Depth               { get; set; }
    public bool           DirectToScreen      { get; set; }
    public uint           Fbo                 { get; set; }
    public Image.Format   ImageFormat         { get; set; }
    public int            InternalColorFormat { get; set; }
    public bool           IsTransparent       { get; set; }
    public RTOverridden   Overridden          { get; set; } = new();
    public uint[]         SdfTextureProcess   { get; } = new uint[2] { 0, 0 };
    public uint           SdfTextureRead      { get; set; }
    public uint           SdfTextureWrite     { get; set; }
    public uint           SdfTextureWriteFb   { get; set; }
    public Vector2D<int>  Size                { get; set; }
    public Guid           Texture             { get; set; }
    public bool           UsedInFrame         { get; set; }
    public uint           ViewCount           { get; set; }
}

internal record RTOverridden
{
    public Guid                            Color        { get; set; }
    public Guid                            Depth        { get; set; }
    public Dictionary<uint, FBOCacheEntry> FboCache     { get; } = new();
    public bool                            IsOverridden { get; set; }
    public Guid                            Velocity     { get; set; }
}

internal record FBOCacheEntry
{
    public uint          Fbo                     { get; set; }
    public Vector2D<int> Size                    { get; set; }
    public List<uint>    AllocatedTextures       { get; set; } = new();
    public int           AllocatedTexturesLength { get; internal set; }
}
