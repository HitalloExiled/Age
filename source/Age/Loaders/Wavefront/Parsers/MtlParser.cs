

using Age.Loaders.Wavefront.Exceptions;

namespace Age.Loaders.Wavefront.Parsers;

public partial class MtlParser(string filepath, StreamReader reader) : Parser(reader)
{
    private readonly string  filepath = filepath;
    private readonly Context context  = new();

    private Token NextToken()
    {
        var unrestricted = this.Lookahead.Type == TokenType.Identifier && this.Lookahead.Value == "newmtl";

        return this.NextToken(unrestricted);
    }

    private void ParseTextureMap()
    {
        var isMap  = this.Lookahead.Value.StartsWith("map_");
        var isRefl = this.Lookahead.Value.StartsWith("refl");
        var isBump = this.Lookahead.Value.StartsWith("bump");

        if (!isMap && !isRefl && !isBump)
        {
            return;
        }

        var textureMapType = this.Lookahead.Value switch
        {
            "map_Kd" => TextureMapType.Color,
            "map_Ks" => TextureMapType.Specular,
            "map_Ns" => TextureMapType.SpecularExponent,
            "map_d"  => TextureMapType.Alpha,
            "refl" or "map_refl" => TextureMapType.Reflection,
            "map_Ke" => TextureMapType.Emission,
            "bump" or "map_Bump" or "map_bump" => TextureMapType.Normal,
            "map_Pr" => TextureMapType.Roughness,
            "map_Pm" => TextureMapType.Metallic,
            "map_Ps" => TextureMapType.Sheen,
            _ => TextureMapType.Count,
        };

        if (textureMapType == TextureMapType.Count)
        {
            throw new ParseException($"MTL texture map type not supported: {this.Lookahead.Value}", this.Lookahead.Line, this.Lookahead.Column, this.Lookahead.Index);
        }

        this.NextToken();

        var textureMap = new TextureMap();

        while (this.Match("-"))
        {
            this.NextToken();

            if (this.MatchIdentifier("o"))
            {
                this.NextToken();

                textureMap.Translation = this.ParseVector3(0, defaultValue: 0);
            }
            else if (this.MatchIdentifier("s"))
            {
                this.NextToken();

                textureMap.Scale = this.ParseVector3(0, defaultValue: 1);
            }
            else if (this.MatchIdentifier("bm"))
            {
                this.NextToken();

                this.context.CurrentMaterial.NormalStrength = this.ParseNumber<float>(1);
            }
            else if (this.MatchIdentifier("type"))
            {
                this.NextToken();

                textureMap.ProjectionType = ProjectionType.Sphere;

                if (this.Lookahead.Value != "sphere")
                {
                    throw new ParseException($"only sphere MTL projection type is supported: {this.Lookahead.Value}", this.Lookahead.Line, this.Lookahead.Column, this.Lookahead.Index);
                }

                this.NextToken();
            }
            else if (this.MatchIdentifier("blendu") || this.MatchIdentifier("blendv") || this.MatchIdentifier("clamp") || this.MatchIdentifier("imfchan"))
            {
                this.NextToken();
                this.NextToken();
            }
            else if (this.MatchIdentifier("boost") || this.MatchIdentifier("texres") || this.MatchIdentifier("mult_value"))
            {
                this.NextToken();
                this.ParseNumber<float>();
            }
            else if (this.MatchIdentifier("mm"))
            {
                this.NextToken();
                this.ParseNumber<float>();
                this.ParseNumber<float>();
            }
            else if (this.MatchIdentifier("t"))
            {
                this.NextToken();
                this.ParseVector(3, 1);
            }
            else
            {
                throw UnexpectedTokenError(this.Lookahead);
            }
        }

        if (!this.MatchNewLine())
        {
            this.Restore();
            this.NextToken(true);

            textureMap.ImagePath = Path.GetFullPath(Path.Join(Path.GetDirectoryName(this.filepath), this.NextToken().Value));
        }

        this.context.CurrentMaterial.TextureMaps[textureMapType] = textureMap;
    }

    public IList<Material> Parse()
    {
        while (this.Lookahead.Type != TokenType.Eof)
        {
            if (this.MatchIdentifier("newmtl"))
            {
                this.NextToken();

                this.context.NewMaterial(this.NextToken().Value);
            }
            else if (this.MatchIdentifier("Ns"))
            {
                this.NextToken();

                this.context.CurrentMaterial.SpecularExponent = this.ParseNumber<float>(324);
            }
            else if (this.MatchIdentifier("Ka"))
            {
                this.NextToken();

                this.context.CurrentMaterial.AmbientColor = this.ParseColor(0);
            }
            else if (this.MatchIdentifier("Kd"))
            {
                this.NextToken();

                this.context.CurrentMaterial.Color = this.ParseColor(0.8f);
            }
            else if (this.MatchIdentifier("Ks"))
            {
                this.NextToken();

                this.context.CurrentMaterial.SpecularColor = this.ParseColor(0.5f);
            }
            else if (this.MatchIdentifier("Ke"))
            {
                this.NextToken();

                this.context.CurrentMaterial.EmissionColor = this.ParseColor(0);
            }
            else if (this.MatchIdentifier("Ni"))
            {
                this.NextToken();

                this.context.CurrentMaterial.Ior = this.ParseNumber(1.45f);
            }
            else if (this.MatchIdentifier("d"))
            {
                this.NextToken();

                this.context.CurrentMaterial.Alpha = this.ParseNumber<float>(1);
            }
            else if (this.MatchIdentifier("illum"))
            {
                this.NextToken();

                this.context.CurrentMaterial.IlluminationModel = (IlluminationModel)this.ParseNumber<float>(1);
            }
            else if (this.MatchIdentifier("Pr"))
            {
                this.NextToken();

                this.context.CurrentMaterial.Roughness = this.ParseNumber(0.5f);
            }
            else if (this.MatchIdentifier("Pm"))
            {
                this.NextToken();

                this.context.CurrentMaterial.Metallic = this.ParseNumber<float>(0);
            }
            else if (this.MatchIdentifier("Ps"))
            {
                this.NextToken();

                this.context.CurrentMaterial.Sheen = this.ParseNumber<float>(0);
            }
            else if (this.MatchIdentifier("Pc"))
            {
                this.NextToken();

                this.context.CurrentMaterial.CoatThickness = this.ParseNumber<float>(0);
            }
            else if (this.MatchIdentifier("Pcr"))
            {
                this.NextToken();

                this.context.CurrentMaterial.CoatRoughness = this.ParseNumber<float>(0);
            }
            else if (this.MatchIdentifier("aniso"))
            {
                this.NextToken();

                this.context.CurrentMaterial.Aniso = this.ParseNumber<float>(0);
            }
            else if (this.MatchIdentifier("anisor"))
            {
                this.NextToken();

                this.context.CurrentMaterial.AnisoRotation = this.ParseNumber<float>(0);
            }
            else if (this.MatchIdentifier("Kt") || this.MatchIdentifier("Tf"))
            {
                this.NextToken();

                this.context.CurrentMaterial.TransmitColor = this.ParseColor(0);
            }
            else
            {
                this.ParseTextureMap();
            }

            this.ExpectNewLine();
        }

        return this.context.GetMaterials();
    }

}
