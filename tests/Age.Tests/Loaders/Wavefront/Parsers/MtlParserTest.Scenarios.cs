using Age.Resources.Loaders.Wavefront;
using Age.Resources.Loaders.Wavefront.Exceptions;

namespace Age.Tests.Loaders.Wavefront.Parsers;

public partial class MtlParserTest
{
    public record ValidScenario
    {
        public required string     Source    { get; set; }
        public required Material[] Expected  { get; set; }
        public bool                Skip      { get; set; }
    }

    public record InvalidScenario
    {
        public required string         Source   { get; set; }
        public required ParseException Expected { get; set; }
        public bool                    Skip     { get; set; }
    }

    public static class Scenarios
    {
        private const bool SKIP = false;

        private static readonly ValidScenario[] valid =
        [
            new()
            {
                Source   = "",
                Expected = [],
                Skip     = SKIP,
            },
            new()
            {
                Source   = "# this is a comment",
                Expected = [],
                Skip     = SKIP,
            },
            new()
            {
                Source =
                """
                newmtl my_material
                Ns 250.0
                Ka 1.0 1.0 1.0
                Kd 0.8 0.8 0.8
                Ks 0.5 0.5 0.5
                Ke 0.0 0.0 0.0
                Ni 1.45
                d 1.0
                illum 2
                Pr 0.5
                Pm 0.0
                Ps 0.0
                Pc 0.0
                Pcr 0.03
                aniso 0.0
                anisor 0.0
                Kt 0.5 0.5 0.5
                map_Kd -o 1 1 1 -s 0.5 0.5 0.5 -bm 2 -type sphere diffuse.png
                map_Ks specular.png
                map_Ns ../specular_exponent.png
                map_d alpha.png
                refl reflection.png
                map_Ke emission.png
                bump normal.png
                map_Pr roughness.png
                map_Pm metallic.png
                map_Ps sheen.png

                newmtl my_material.001
                Tf 0.25 0.25 0.25
                map_refl reflection.png
                map_Bump normal.png

                newmtl my_material.002
                map_bump normal.png
                """,
                Expected =
                [
                    new("my_material")
                    {
                        SpecularExponent  = 250,
                        AmbientColor      = new(1, 1, 1),
                        Color             = new(0.8f, 0.8f, 0.8f),
                        SpecularColor     = new(0.5f, 0.5f, 0.5f),
                        EmissionColor     = new(0, 0, 0),
                        Ior               = 1.45f,
                        Alpha             = 1,
                        IlluminationModel = IlluminationModel.HighlightOn,
                        Roughness         = 0.5f,
                        Metallic          = 0,
                        Sheen             = 0,
                        CoatThickness     = 0,
                        CoatRoughness     = 0.03f,
                        Aniso             = 0,
                        AnisoRotation     = 0,
                        NormalStrength    = 2,
                        TransmitColor     = new(0.5f, 0.5f, 0.5f),
                        TextureMaps       =
                        {
                            [TextureMapType.Color] = new()
                            {
                                ImagePath      = GetFullPath("diffuse.png"),
                                ProjectionType = ProjectionType.Sphere,
                                Scale          = new(0.5f, 0.5f, 0.5f),
                                Translation    = new(1, 1, 1),
                            },
                            [TextureMapType.Specular]         = new() { ImagePath = GetFullPath("specular.png") },
                            [TextureMapType.SpecularExponent] = new() { ImagePath = GetFullPath("../specular_exponent.png") },
                            [TextureMapType.Alpha]            = new() { ImagePath = GetFullPath("alpha.png") },
                            [TextureMapType.Reflection]       = new() { ImagePath = GetFullPath("reflection.png") },
                            [TextureMapType.Emission]         = new() { ImagePath = GetFullPath("emission.png") },
                            [TextureMapType.Normal]           = new() { ImagePath = GetFullPath("normal.png") },
                            [TextureMapType.Roughness]        = new() { ImagePath = GetFullPath("roughness.png") },
                            [TextureMapType.Metallic]         = new() { ImagePath = GetFullPath("metallic.png") },
                            [TextureMapType.Sheen]            = new() { ImagePath = GetFullPath("sheen.png") },
                        },
                    },
                    new("my_material.001")
                    {
                        TransmitColor = new(0.25f, 0.25f, 0.25f),
                        TextureMaps =
                        {
                            [TextureMapType.Reflection] = new() { ImagePath = GetFullPath("reflection.png") },
                            [TextureMapType.Normal]     = new() { ImagePath = GetFullPath("normal.png") }
                        },
                    },
                    new("my_material.002")
                    {
                        TextureMaps =
                        {
                            [TextureMapType.Normal] = new() { ImagePath = GetFullPath("normal.png") }
                        },
                    },
                ],
                Skip = SKIP,
            },
            new()
            {
                Source =
                """
                newmtl my_material
                map_Kd -blendu on -blendv off -boost 1.0 -mm 1 2 -t 1 2 3 -texres 0.5 -clamp off -imfchan r tex.tga
                """,
                Expected =
                [
                    new("my_material")
                    {
                        TextureMaps =
                        {
                            [TextureMapType.Color] = new() { ImagePath = GetFullPath("tex.tga") },
                        },
                    },
                ],
                Skip = SKIP,
            },
        ];

        private static readonly InvalidScenario[] invalid =
        [
            new()
            {
                Source   = "foo",
                Expected = new("Unexpected token foo", 1, 1, 0),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "map_",
                Expected = new("MTL texture map type not supported: map_", 1, 1, 0),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "map_Kd -type box",
                Expected = new("only sphere MTL projection type is supported: box", 1, 14, 13),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "map_Kd -x",
                Expected = new("Unexpected token x", 1, 9, 8),
                Skip     = SKIP,
            },
        ];

        public static TheoryData<ValidScenario>   Valid   => new(valid);
        public static TheoryData<InvalidScenario> Invalid => new(invalid);
    }
}
