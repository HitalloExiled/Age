using Age.Numerics;

namespace Age.Resources.Loaders.Wavefront.Parsers;

public partial class ObjParser(string filepath, StreamReader reader, MtlLoader mtlLoader, ObjParser.Options? options = null) : Parser(reader)
{
    private readonly string  filepath = filepath;
    private readonly Options options  = options ?? new();
    private readonly Context context  = new(Path.GetFileNameWithoutExtension(filepath));

    private Token NextToken()
    {
        var unrestricted = this.Lookahead.Type == TokenType.Identifier && this.Lookahead.Value is "g" or "o" or "usemtl" or "mtllib";

        return this.NextToken(unrestricted);
    }

    private Face ParseFace()
    {
        this.NextToken();

        var vertexIndices = new List<VertexData>();
        var triples       = new[]
        {
            this.context.Vertices.Count,
            this.context.TexCoords.Count,
            this.context.Normals.Count
        };

        while (this.Lookahead.Type == TokenType.NumericLiteral || this.Match("-"))
        {
            var values = new int[3];

            Array.Fill(values, -1);

            for (var i = 0; i < 3; i++)
            {
                if (this.TryParseNumber<int>(out var index))
                {
                    values[i] = (index < 0 ? triples[i] + index : index) - 1;
                }

                if (i < 2)
                {
                    if (!this.Match("/"))
                    {
                        break;
                    }

                    this.NextToken();
                }
            }

            var vertexIndex    = values[0];
            var textCoordIndex = values[1];
            var normalIndex    = values[2];
            var colorIndex     = this.context.GetColorIndex(vertexIndex);

            if (textCoordIndex > -1)
            {
                this.context.SetTexCoordIndex(vertexIndex, textCoordIndex);
            }

            if (normalIndex > -1)
            {
                this.context.SetVertexNormalIndex(vertexIndex, normalIndex);
            }

            vertexIndices.Add(new(vertexIndex, textCoordIndex, normalIndex, colorIndex));
        }

        return new()
        {
            Group        = this.context.GroupIndex,
            Indices      = vertexIndices,
            ShadedSmooth = this.context.ShadedSmooth,
            Material     = this.context.CurrentMaterial
        };
    }

    private Line ParseLine()
    {
        var triples = new[]
        {
            this.context.Vertices.Count,
            this.context.TexCoords.Count,
            this.context.Normals.Count
        };

        this.NextToken();

        var values = new int[2];

        for (var i = 0; i < 2; i++)
        {
            values[i] = this.TryParseNumber<int>(out var index)
                ? (index < 0 ? triples[i] + index : index) - 1
                : throw UnexpectedTokenError(this.Lookahead);
        }

        return new(values[0], values[1], this.context.GroupIndex);
    }

    private string ParseName()
    {
        this.NextToken();

        return this.NextToken().Value!;
    }

    private Vector3<float> ParseNormal()
    {
        this.NextToken();

        return this.ParseVector3();
    }

    private bool ParseSmoothGroup()
    {
        this.NextToken();

        var token = this.Lookahead;

        if (this.Lookahead.Type == TokenType.NumericLiteral && this.TryParseNumber<int>(out var value))
        {
            return value != 0;
        }
        else if (this.MatchIdentifier("off") || this.MatchIdentifier("null"))
        {
            this.NextToken();

            return false;
        }

        throw UnexpectedTokenError(token);
    }

    private Vector3<float> ParseTextCoord()
    {
        this.NextToken();

        return this.ParseVector3(1, 0, 1);
    }

    private (Vector4<float> Vertex, Color? Color) ParseVertexWithColor()
    {
        this.NextToken();

        var values = this.ParseVector(6, 3, out var parsed);

        var vertices = new Vector4<float>(values[0], values[1], values[2], parsed == 4 ? values[3] : 1);
        var colors   = parsed == 6 ? new Color(values[3], values[4], values[5]) : default(Color?);

        return (vertices, colors);
    }

    public Data Parse()
    {
        while (this.Lookahead.Type != TokenType.Eof)
        {
            if (!this.MatchNewLine())
            {
                if (this.MatchIdentifier("g"))
                {
                    var name = this.ParseName();

                    this.context.NewGroup(name);

                    if (this.options.SplitByGroup)
                    {
                        this.context.NewObject(name);
                    }
                }
                else if (this.MatchIdentifier("o"))
                {
                    var name = this.ParseName();

                    if (this.options.SplitByObject)
                    {
                        this.context.NewObject(name);
                    }
                }
                else if (this.MatchIdentifier("v"))
                {
                    var (vertex, color) = this.ParseVertexWithColor();

                    this.context.AddVertexWithColor(vertex, color);
                }
                else if (this.MatchIdentifier("vn"))
                {
                    this.context.AddNormal(this.ParseNormal());
                }
                else if (this.MatchIdentifier("vt"))
                {
                    this.context.AddTexCoord(this.ParseTextCoord());
                }
                else if (this.MatchIdentifier("f"))
                {
                    this.context.AddFace(this.ParseFace());
                }
                else if (this.MatchIdentifier("l"))
                {
                    this.context.AddLine(this.ParseLine());
                }
                else if (this.MatchIdentifier("mtllib"))
                {
                    var mtlPath = this.ParseName();

                    if (mtlPath.Length > 2 && mtlPath.StartsWith('"') && mtlPath.EndsWith('"'))
                    {
                        mtlPath = mtlPath[1..^1];
                    }

                    var path = Path.GetFullPath(Path.Join(Path.GetDirectoryName(this.filepath), mtlPath));

                    if (mtlLoader.TryLoad(path, out var materials))
                    {
                        this.context.AddMaterials(materials);
                    }
                }
                else if (this.MatchIdentifier("usemtl"))
                {
                    this.context.SetCurrentMaterial(this.ParseName());
                }
                else
                {
                    this.context.ShadedSmooth = this.MatchIdentifier("s")
                        ? this.ParseSmoothGroup()
                        : throw UnexpectedTokenError(this.Lookahead);
                }
            }

            this.ExpectNewLine();
        }

        return this.context.ToData();
    }
}
