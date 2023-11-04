using Age.Numerics;

namespace Age.Resources.Loaders.Wavefront;

public record Material(string Name)
{
    public static Material Default { get; } = new("default");

    /// <summary>
    /// d
    /// </summary>
    public float? Alpha { get; set; }

    /// <summary>
    /// Ka
    /// </summary>
    public Color? AmbientColor { get; set; }

    /// <summary>
    /// aniso
    /// </summary>
    public float? Aniso { get; set; }

    /// <summary>
    /// anisor
    /// </summary>
    public float? AnisoRotation { get; set; }

    /// <summary>
    /// Pcr
    /// </summary>
    public float? CoatRoughness { get; set; }

    /// <summary>
    /// Pc
    /// </summary>
    public float? CoatThickness { get; set; }

    /// <summary>
    /// Kd
    /// </summary>
    public Color? Color { get; set; }

    /// <summary>
    /// Ke
    /// </summary>
    public Color? EmissionColor  { get; set; }

    public IlluminationModel? IlluminationModel { get; set; }

    /// <summary>
    /// Ni
    /// </summary>
    public float? Ior { get; set; }

    /// <summary>
    /// Pm
    /// </summary>
    public float? Metallic { get; set; }

    public float? NormalStrength { get; set; }

    /// <summary>
    /// Pr
    /// </summary>
    public float? Roughness { get; set; }

    /// <summary>
    /// Ps
    /// </summary>
    public float? Sheen { get; set; }

    /// <summary>
    /// Ks
    /// </summary>
    public Color? SpecularColor { get; set; }

    /// <summary>
    /// Ns
    /// </summary>
    public float? SpecularExponent { get; set; }

    public Dictionary<TextureMapType, TextureMap> TextureMaps { get; set; } = [];

    /// <summary>
    /// Kt
    /// </summary>
    public Color? TransmitColor { get; set; }
}
