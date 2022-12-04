using Age.Core.IO;
using Silk.NET.OpenGLES;

namespace Age.Drivers.GLES3;

internal record Texture(Guid Id = default)
{
    public enum TypeEnum
    {
        T2D,
        LAYERED,
        T3D
	    };

    public Guid         Id                  { get; } = Id;
    public bool         Active              { get; set; }
    public int          AllocHeight         { get; set; }
    public int          AllocWidth          { get; set; }
    public Image.Format Format              { get; set; }
    public GLEnum       FormatCache         { get; set; }
    public int          Height              { get; set; }
    public GLEnum       InternalFormatCache { get; set; }
    public bool         IsProxy             { get; set; }
    public bool         IsRenderTarget      { get; set; }
    public uint         Layers              { get; set; }
    public Guid         ProxyTo             { get; set; }
    public Image.Format RealFormat          { get; set; }
    public GLEnum       Target              { get; set; }
    public uint         TextId              { get; set; }
    public TypeEnum     Type                { get; set; }
    public GLEnum       TypeCache           { get; set; }
    public int          Width               { get; set; }
}
