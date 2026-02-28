
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Age.Core.Collections;
using Age.Numerics;
using SkiaSharp;

namespace ThirdParty.Skia.Svg;

public partial class SKSvg(float pixelsPerInch, SKSize canvasSize)
{
    private static readonly Regex                 clipPathUrlPattern = CreateClipPathUrlPattern();
    private static readonly Regex                 fillUrlPattern     = CreateFillUrlPattern();
    private static readonly Regex                 groupPattern       = CreateGroupPattern();
    private static readonly Regex                 keyValuePattern    = CreateKeyValuePattern();
    private static readonly Regex                 percPattern        = CreatePercPattern();
    private readonly Dictionary<string, XElement> references         = [];
    private static readonly XNamespace            svg                = "http://www.w3.org/2000/svg";
    private static readonly Regex                 unitPattern        = CreateUnitPattern();
    private static readonly Regex                 wsPattern          = CreateWSPattern();
    private static readonly XNamespace            xlink              = "http://www.w3.org/1999/xlink";

    public float  PixelsPerInch { get; } = pixelsPerInch;

    public SKSize    CanvasSize  { get; private set; } = canvasSize;
    public string?   Description { get; private set; }
    public SKPicture Picture     { get; private set; } = null!;
    public string?   Title       { get; private set; }
    public string?   Version     { get; private set; }
    public SKRect    ViewBox     { get; private set; }

    public bool ThrowOnUnsupportedElement { get; init; }

    private readonly XmlReaderSettings xmlReaderSettings = new()
    {
        DtdProcessing  = DtdProcessing.Ignore,
        IgnoreComments = true
    };

    public SKSvg() : this(160f, SKSize.Empty)
    { }

    public SKSvg(float pixelsPerInch) : this(pixelsPerInch, SKSize.Empty)
    { }

    public SKSvg(SKSize canvasSize) : this(160f, canvasSize)
    { }

    [GeneratedRegex("url\\s*\\(\\s*#([^\\)]+)\\)")]
    private static partial Regex CreateClipPathUrlPattern();

    [GeneratedRegex("url\\s*\\(\\s*#([^\\)]+)\\)")]
    private static partial Regex CreateFillUrlPattern();

    [GeneratedRegex("\\s*([\\w-]+)\\s*:\\s*(.*)")]
    private static partial Regex CreateKeyValuePattern();

    [GeneratedRegex(@"(?<!\\)\((?!\?:)[^[\]]*?(?<!\\)\)")]
    private static partial Regex CreateGroupPattern();

    [GeneratedRegex("%")]
    private static partial Regex CreatePercPattern();

    [GeneratedRegex("px|pt|em|ex|pc|cm|mm|in")]
    private static partial Regex CreateUnitPattern();

    [GeneratedRegex("\\s{2,}")]
    private static partial Regex CreateWSPattern();

    private static SKPaint CreatePaint(bool stroke = false) =>
        new()
        {
            IsAntialias = true,
            IsStroke = stroke,
            Color = SKColors.Black
        };

    private static XmlParserContext CreateSvgXmlContext()
    {
        var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
        xmlNamespaceManager.AddNamespace(string.Empty, svg.NamespaceName);
        xmlNamespaceManager.AddNamespace("xlink", xlink.NamespaceName);
        return new XmlParserContext(null, xmlNamespaceManager, null, XmlSpace.None);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<char> GetString(Dictionary<string, string> style, ReadOnlySpan<char> name, ReadOnlySpan<char> defaultValue = "") =>
        style.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(name, out var value) ? value : defaultValue;

    private static bool HasSvgNamespace(XName name) =>
        string.IsNullOrEmpty(name.Namespace?.NamespaceName) || name.Namespace == svg || name.Namespace == xlink;

    private static SKFontStyleSlant ReadFontStyle(Dictionary<string, string> fontStyle, SKFontStyleSlant defaultStyle = SKFontStyleSlant.Upright)
    {
        var result = defaultStyle;

        if (fontStyle.TryGetValue("font-style", out var value) && !string.IsNullOrWhiteSpace(value))
        {
            result = value switch
            {
                "italic"  => SKFontStyleSlant.Italic,
                "oblique" => SKFontStyleSlant.Oblique,
                "normal"  => SKFontStyleSlant.Upright,
                _ => defaultStyle,
            };
        }

        return result;
    }

    private static int ReadFontWeight(Dictionary<string, string> fontStyle, int defaultWeight = 400)
    {
        var result = defaultWeight;
        if (fontStyle.TryGetValue("font-weight", out var value) && !string.IsNullOrWhiteSpace(value) && !int.TryParse(value, out result))
        {
            result = value switch
            {
                "normal"  => 400,
                "bold"    => 700,
                "bolder"  => result + 100,
                "lighter" => result - 100,
                _ => defaultWeight,
            };
        }

        return Math.Min(Math.Max(100, result), 1000);
    }

    private static int ReadFontWidth(Dictionary<string, string> fontStyle, int defaultWidth = 5)
    {
        var result = defaultWidth;
        if (fontStyle.TryGetValue("font-stretch", out var value) && !string.IsNullOrWhiteSpace(value) && !int.TryParse(value, out result))
        {
            result = value switch
            {
                "ultra-condensed" => 1,
                "extra-condensed" => 2,
                "condensed"       => 3,
                "semi-condensed"  => 4,
                "normal"          => 5,
                "semi-expanded"   => 6,
                "expanded"        => 7,
                "extra-expanded"  => 8,
                "ultra-expanded"  => 9,
                "wider"           => result + 1,
                "narrower"        => result - 1,
                _ => defaultWidth,
            };
        }

        return Math.Min(Math.Max(1, result), 9);
    }

    private static Dictionary<string, string> ReadStyle(ReadOnlySpan<char> style)
    {
        var dictionary = new Dictionary<string, string>();

        foreach (var range in style.Split(';'))
        {
            var content = style[range];

            if (!content.IsEmpty)
            {
                var enumerator = content.Split(':');

                enumerator.MoveNext();

                var key = content[enumerator.Current].Trim();

                enumerator.MoveNext();

                var value = content[enumerator.Current].Trim();

                dictionary[new(key)] = new(value);
            }
        }

        return dictionary;
    }

    private static Dictionary<string, string> ReadStyle(XElement element)
    {
        var style = new Dictionary<string, string>();

        foreach (var attribute in element.Attributes())
        {
            if (HasSvgNamespace(attribute.Name))
            {
                style[attribute.Name.LocalName] = attribute.Value;
            }
        }

        var rawStyle = element.Attribute("style")?.Value;

        if (!string.IsNullOrWhiteSpace(rawStyle))
        {
            foreach (var item in ReadStyle(rawStyle))
            {
                style[item.Key] = item.Value;
            }
        }

        return style;
    }

    private static SKTextAlign ReadTextAlignment(XElement element)
    {
        ReadOnlySpan<char> text = default;

        if (element != null)
        {
            var textAnchor = element.Attribute("text-anchor");

            if (textAnchor != null && !string.IsNullOrWhiteSpace(textAnchor.Value))
            {
                text = textAnchor.Value;
            }
            else
            {
                var style = element.Attribute("style");
                if (style != null && !string.IsNullOrWhiteSpace(style.Value))
                {
                    text = GetString(ReadStyle(style.Value), "text-anchor");
                }
            }
        }

        return text != "end" ? text == "middle" ? SKTextAlign.Center : SKTextAlign.Left : SKTextAlign.Right;
    }

    private static NativeRefArray<byte> ReadUriBytes(ReadOnlySpan<char> uri)
    {
        if (!uri.IsEmpty)
        {
            var index = uri.IndexOf(",");

            if (index != -1 && index - 1 < uri.Length)
            {
                uri = uri[(index + 1)..];

                var bytesLenght = (uri.Length * 3 / 4) - 2;

                var bytes = new NativeRefArray<byte>(bytesLenght);

                if (Convert.TryFromBase64Chars(uri, bytes, out var bytesWritten))
                {
                    bytes.Resize(bytesWritten);

                    return bytes;
                }
            }
        }

        return default;
    }

    private float ReadBaselineShift(XElement element)
    {
        ReadOnlySpan<char> raw = default;
        if (element != null)
        {
            var baselineShift = element.Attribute("baseline-shift");
            if (baselineShift != null && !string.IsNullOrWhiteSpace(baselineShift.Value))
            {
                raw = baselineShift.Value;
            }
            else
            {
                var style = element.Attribute("style");
                if (style != null && !string.IsNullOrWhiteSpace(style.Value))
                {
                    raw = GetString(ReadStyle(style.Value), "baseline-shift");
                }
            }
        }

        return this.ReadNumber(raw);
    }

    private SKCircle ReadCircle(XElement element)
    {
        var x = this.ReadNumber(element.Attribute("cx")!);
        var y = this.ReadNumber(element.Attribute("cy")!);

        return new SKCircle(new SKPoint(x, y), this.ReadNumber(element.Attribute("r")!));
    }

    private SKPath? ReadClipPath(ReadOnlySpan<char> raw)
    {
        if (raw.IsEmpty)
        {
            return null;
        }

        SKPath? sKPath = null;
        var flag = false;

        var enumerator = clipPathUrlPattern.EnumerateMatches(raw);

        if (enumerator.MoveNext())
        {
            var content = raw.Slice(enumerator.Current);

            var groupEnumerator = groupPattern.EnumerateMatches(content);

            groupEnumerator.MoveNext();

            var text = content.Slice(groupEnumerator.Current).Trim();

            if (this.references.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(text, out var value))
            {
                sKPath = this.ReadClipPathDefinition(value);

                if (sKPath != null)
                {
                    flag = true;
                }
            }
            else
            {
                this.LogOrThrow($"Invalid clip-path url reference: {text}");
            }
        }

        if (!flag)
        {
            this.LogOrThrow($"Unsupported clip-path: {raw}");
        }

        return sKPath;
    }

    private SKPath? ReadClipPathDefinition(XElement element)
    {
        if (element.Name.LocalName != "clipPath" || !element.HasElements)
        {
            return null;
        }

        var path = new SKPath();

        foreach (var child in element.Elements())
        {
            var childPath = this.ReadElement(child);

            if (childPath != null)
            {
                path.AddPath(childPath);
            }
            else
            {
                this.LogOrThrow($"SVG element '{child.Name.LocalName}' is not supported in clipPath.");
            }
        }

        return path;
    }

    private SKPath? ReadElement(XElement element)
    {
        var sKPath = new SKPath();
        var localName = element.Name.LocalName;
        switch (localName)
        {
            case "rect":
                {
                    var sKRoundedRect = this.ReadRoundedRect(element);
                    if (sKRoundedRect.IsRounded)
                    {
                        sKPath.AddRoundRect(sKRoundedRect.Rect, sKRoundedRect.RadiusX, sKRoundedRect.RadiusY, SKPathDirection.Clockwise);
                    }
                    else
                    {
                        sKPath.AddRect(sKRoundedRect.Rect);
                    }

                    break;
                }
            case "ellipse":
                sKPath.AddOval(this.ReadOval(element).BoundingRect);
                break;
            case "circle":
                {
                    var sKCircle = this.ReadCircle(element);
                    sKPath.AddCircle(sKCircle.Center.X, sKCircle.Center.Y, sKCircle.Radius);
                    break;
                }
            case "path":
                {
                    var d = element.Attribute("d")?.Value;
                    if (!string.IsNullOrWhiteSpace(d))
                    {
                        sKPath.Dispose();
                        sKPath = SKPath.ParseSvgPathData(d);
                    }

                    break;
                }
            case "polygon":
            case "polyline":
                {
                    var flag = localName == "polygon";
                    var text = element.Attribute("points")?.Value;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        text = "M" + text;
                        if (flag)
                        {
                            text += " Z";
                        }

                        sKPath.Dispose();
                        sKPath = SKPath.ParseSvgPathData(text);
                    }

                    break;
                }
            case "line":
                {
                    var sKLine = this.ReadLine(element);
                    sKPath.MoveTo(sKLine.P1);
                    sKPath.LineTo(sKLine.P2);
                    break;
                }
            default:
                sKPath.Dispose();
                sKPath = null;
                break;
        }

        return sKPath;
    }

    private void ReadElement(XElement element, SKCanvas canvas, SKFont font, SKPaint? parentFillPaint, SKPaint? parentStrokePaint)
    {
        if (element.Attribute("display")?.Value == "none")
        {
            return;
        }

        var matrix = this.ReadTransform(element.Attribute("transform")?.Value ?? string.Empty);

        canvas.Save();
        canvas.Concat(in matrix);

        var clipPath = this.ReadClipPath(element.Attribute("clip-path")?.Value ?? string.Empty);

        if (clipPath != null)
        {
            canvas.ClipPath(clipPath);
        }

        var localName = element.Name.LocalName;
        var isGroup   = localName == "g";
        var style     = this.ReadPaints(element, isGroup, parentFillPaint, parentStrokePaint, out var fillPaint, out var strokePaint);

        switch (localName)
        {
            case "image":
                {
                    using var svgImage = this.ReadImage(element);

                    if (svgImage.Bytes.IsEmpty)
                    {
                        break;
                    }

                    using var bitmap = SKBitmap.Decode(svgImage.Bytes);

                    if (bitmap != null)
                    {
                        canvas.DrawBitmap(bitmap, svgImage.Rect);
                    }

                    break;
                }
            case "text":
                if (fillPaint != null || strokePaint != null)
                {
                    using var svgText = this.ReadText(element, font, fillPaint, strokePaint);

                    var currentX = svgText.Location.X;
                    var currentY = svgText.Location.Y;

                    var textWidth = svgText.MeasureTextWidth();

                    switch (svgText.TextAlign)
                    {
                        case SKTextAlign.Left:
                            break;
                        case SKTextAlign.Center:
                            currentX -= textWidth / 2;
                            break;
                        case SKTextAlign.Right:
                            currentX -= textWidth;
                            break;
                    }

                    foreach (var span in svgText.Spans)
                    {
                        currentY = span.Y ?? currentY;
                        currentX = span.X ?? currentX;

                        if (span.TextPath != null)
                        {
                            canvas.DrawTextOnPath(span.Text, span.TextPath.Path, span.TextPath.StartOffset ?? 0, 0, span.Font, span.Fill ?? span.Stroke);
                        }
                        else
                        {
                            canvas.DrawText(span.Text, currentX, currentY - span.BaselineShift, span.Font, span.Fill ?? span.Stroke);
                        }

                        currentX += span.TextWidth;
                    }
                }

                break;
            case "rect":
            case "ellipse":
            case "circle":
            case "path":
            case "polygon":
            case "polyline":
            case "line":
                {
                    if (strokePaint == null && fillPaint == null)
                    {
                        break;
                    }

                    var path = this.ReadElement(element);

                    if (path != null)
                    {
                        if (fillPaint != null)
                        {
                            canvas.DrawPath(path, fillPaint);
                        }

                        if (strokePaint != null)
                        {
                            canvas.DrawPath(path, strokePaint);
                        }
                    }

                    break;
                }
            case "g":
                {
                    if (!element.HasElements)
                    {
                        break;
                    }

                    var opacity = this.ReadOpacity(style);
                    if (opacity != 1f)
                    {
                        var alphaPaint = new SKPaint
                        {
                            Color = SKColors.Black.WithAlpha((byte)(255f * opacity))
                        };
                        canvas.SaveLayer(alphaPaint);
                    }

                    foreach (var child in element.Elements())
                    {
                        this.ReadElement(child, canvas, font, fillPaint, strokePaint);
                    }

                    if (opacity != 1f)
                    {
                        canvas.Restore();
                    }

                    break;
                }
            case "use":
                if (element.HasAttributes)
                {
                    var referencedElement = this.ReadHref(element);
                    if (referencedElement != null)
                    {
                        var x                 = this.ReadNumber(element.Attribute("x")!);
                        var y                 = this.ReadNumber(element.Attribute("y")!);
                        var translationMatrix = SKMatrix.CreateTranslation(x, y);

                        canvas.Save();
                        canvas.Concat(in translationMatrix);

                        this.ReadElement(referencedElement, canvas, font, strokePaint, fillPaint);

                        canvas.Restore();
                    }
                }

                break;
            case "switch":
                if (!element.HasElements)
                {
                    break;
                }

                foreach (var child in element.Elements())
                {
                    var requiredFeatures   = child.Attribute("requiredFeatures");
                    var requiredExtensions = child.Attribute("requiredExtensions");
                    var systemLanguage     = child.Attribute("systemLanguage");

                    if (requiredFeatures == null && requiredExtensions == null && systemLanguage == null)
                    {
                        this.ReadElement(child, canvas, font, strokePaint, fillPaint);
                    }
                }

                break;
            default:
                this.LogOrThrow($"SVG element '{localName}' is not supported");
                break;
            case "defs":
            case "title":
            case "desc":
            case "description":
                break;
        }

        canvas.Restore();

        fillPaint?.Dispose();
        strokePaint?.Dispose();
    }

    private SKSvgImage ReadImage(XElement element)
    {
        var x      = this.ReadNumber(element.Attribute("x")!);
        var y      = this.ReadNumber(element.Attribute("y")!);
        var width  = this.ReadNumber(element.Attribute("width")!);
        var height = this.ReadNumber(element.Attribute("height")!);
        var rect   = SKRect.Create(x, y, width, height);

        NativeRefArray<byte> bytes = default;

        var text = ReadHrefString(element);

        if (!text.IsEmpty)
        {
            if (text.StartsWith("data:"))
            {
                bytes = ReadUriBytes(text);
            }
            else
            {
                this.LogOrThrow("Remote images are not supported");
            }
        }

        return new SKSvgImage(rect, text, bytes);
    }

    private SKLine ReadLine(XElement element)
    {
        var x  = this.ReadNumber(element.Attribute("x1")!);
        var x2 = this.ReadNumber(element.Attribute("x2")!);
        var y  = this.ReadNumber(element.Attribute("y1")!);

        return new SKLine(new SKPoint(x, y), new SKPoint(x2, this.ReadNumber(element.Attribute("y2")!)));
    }

    private SKOval ReadOval(XElement element)
    {
        var x = this.ReadNumber(element.Attribute("cx")!);
        var y = this.ReadNumber(element.Attribute("cy")!);

        return new SKOval(new SKPoint(x, y), this.ReadNumber(element.Attribute("rx")!), this.ReadNumber(element.Attribute("ry")!));
    }

    private SKRoundedRect ReadRoundedRect(XElement element)
    {
        var x      = this.ReadNumber(element.Attribute("x")!);
        var y      = this.ReadNumber(element.Attribute("y")!);
        var width  = this.ReadNumber(element.Attribute("width")!);
        var height = this.ReadNumber(element.Attribute("height")!);

        var num  = this.ReadOptionalNumber(element.Attribute("rx")!);
        var num2 = this.ReadOptionalNumber(element.Attribute("ry")!);

        return new SKRoundedRect(SKRect.Create(x, y, width, height), num ?? num2 ?? 0f, num2 ?? num ?? 0f);
    }

    private SKText ReadText(XElement element, SKFont parentFont, SKPaint? fillPaint, SKPaint? strokePaint)
    {
        var x = this.ReadNumber(element.Attribute("x")!);
        var y = this.ReadNumber(element.Attribute("y")!);

        var xy = new SKPoint(x, y);

        var textAlign     = ReadTextAlignment(element);
        var baselineShift = this.ReadBaselineShift(element);

        this.ReadFontAttributes(element, parentFont, out var font);

        return this.ReadTextSpans(element, xy, font, textAlign, baselineShift, fillPaint, strokePaint);
    }

    private SKText ReadTextSpans(XElement element, SKPoint xy, SKFont font, SKTextAlign textAlign, float baselineShift, SKPaint? parentFillPaint, SKPaint? parentStrokePaint)
    {
        var skText               = new SKText(xy, textAlign);
        var currentBaselineShift = baselineShift;

        var nodes = element.Nodes().ToArray();

        for (var i = 0; i < nodes.Length; i++)
        {
            var node    = nodes[i];
            var isFirst = i == 0;
            var isLast  = i == nodes.Length - 1;

            if (node.NodeType == XmlNodeType.Text)
            {
                using var lines = new NativeStringList();

                foreach (var line in ((XText)node).Value.AsSpan().EnumerateLines())
                {
                    lines.Add(new(line));
                }

                if (lines.Count > 0)
                {
                    if (isFirst)
                    {
                        lines[0] = lines[0].TrimStart([]);
                    }

                    if (isLast)
                    {
                        lines[^1] = lines[^1].TrimEnd([]);
                    }

                    var text = wsPattern.Replace(string.Concat(lines), " ");

                    skText.Append(new SKTextSpan(text, font, parentFillPaint, parentStrokePaint, null, null, currentBaselineShift, null));
                }
            }
            else if (node.NodeType == XmlNodeType.Element)
            {
                var tspanElement = (XElement)node;

                if (tspanElement.Name.LocalName == "tspan")
                {
                    var tspanX     = this.ReadOptionalNumber(tspanElement.Attribute("x")!);
                    var tspanY     = this.ReadOptionalNumber(tspanElement.Attribute("y")!);

                    this.ReadFontAttributes(tspanElement, font, out var spanFont);

                    currentBaselineShift = this.ReadBaselineShift(tspanElement);

                    this.ReadPaints(tspanElement, false, parentFillPaint, parentStrokePaint, out var fillPaint, out var strokePaint);

                    skText.Append(new SKTextSpan(tspanElement.Value, spanFont, fillPaint, strokePaint, tspanX, tspanY, currentBaselineShift, null));
                }
                else if (tspanElement.Name.LocalName == "textPath")
                {
                    var href = this.ReadHref(tspanElement);

                    if (href != null)
                    {
                        var d = href.Attribute("d")?.Value;

                        if (!string.IsNullOrWhiteSpace(d))
                        {
                            var text            = tspanElement.Value;
                            var path            = SKPath.ParseSvgPathData(d);
                            var lengthAdjust    = tspanElement.Attribute("lengthAdjust")?.Value;
                            var method          = tspanElement.Attribute("method")?.Value;
                            var spacing         = tspanElement.Attribute("spacing")?.Value;
                            var startOffsetUnit = this.ReadOptionalUnit(tspanElement.Attribute("startOffset")!);
                            var textLengthUnit  = this.ReadOptionalUnit(tspanElement.Attribute("textLength")!);

                            float? startOffset = null;
                            float? textLength  = null;

                            using var pathMeasure = new SKPathMeasure(path);

                            var pathLength = pathMeasure.Length / 2;

                            if (startOffsetUnit?.TryGetPercentage(out var startOffsetPc) == true)
                            {
                                startOffset = pathLength * startOffsetPc;
                            }
                            else if (startOffsetUnit?.TryGetPixel(out var startOffsetPx) == true)
                            {
                                startOffset = startOffsetPx / 2;
                            }

                            if (textLengthUnit?.TryGetPercentage(out var textLengthPc) == true)
                            {
                                textLength = pathLength * textLengthPc;
                            }
                            else if (textLengthUnit?.TryGetPixel(out var textLengthPx) == true)
                            {
                                textLength = textLengthPx;
                            }

                            var textPath = new SkTextSpanPath(
                                path,
                                lengthAdjust,
                                method,
                                spacing,
                                startOffset,
                                textLength
                            );

                            skText.Append(new SKTextSpan(text, font, parentFillPaint, parentStrokePaint, null, null, currentBaselineShift, textPath));
                        }
                    }
                }
                else
                {
                    this.LogOrThrow($"Unsuported element {tspanElement.Name.LocalName}.");
                }
            }
        }

        return skText;
    }

    private void ReadFontAttributes(XElement element, SKFont? parentFont, out SKFont font)
    {
        var style = ReadStyle(element);

        var slant  = ReadFontStyle(style,  SKFontStyleSlant.Upright);
        var weight = ReadFontWeight(style, parentFont?.Typeface.FontWeight ?? 400);
        var width  = ReadFontWidth(style,  parentFont?.Typeface.FontWidth ?? 5);

        var familyName = style.TryGetValue("font-family", out var value)
            ? value
            : parentFont?.Typeface.FamilyName ?? SKTypeface.Default.FamilyName;

        font = new(SKTypeface.FromFamilyName(familyName, weight, width, slant));

        if (style.TryGetValue("font-size", out var fontSize))
        {
            font.Size = this.ReadNumber(fontSize);
        }
        else if (parentFont != null)
        {
            font.Size = parentFont.Size;
        }
    }

    private SKMatrix ReadTransform(ReadOnlySpan<char> raw)
    {
        var result = SKMatrix.CreateIdentity();

        if (raw.IsEmpty)
        {
            return result;
        }

        Span<Range> rangesAllocation = stackalloc Range[7];

        foreach (var range in raw.Trim().Split(')'))
        {
            var slice  = raw[range].Trim();
            var matrix = SKMatrix.CreateIdentity();

            var count = 0;

            foreach (var subRange in slice.SplitAny(['(', ',', ' ', '\t', '\r', '\n']))
            {
                rangesAllocation[0] = subRange;
                count++;
            }

            var slices = rangesAllocation[..count];

            switch (raw[slices[0]])
            {
                case "matrix":
                    if (slices.Length == 7)
                    {
                        matrix.Values =
                        [
                            this.ReadNumber(raw[slices[1]]),
                            this.ReadNumber(raw[slices[3]]),
                            this.ReadNumber(raw[slices[5]]),
                            this.ReadNumber(raw[slices[2]]),
                            this.ReadNumber(raw[slices[4]]),
                            this.ReadNumber(raw[slices[6]]),
                            0f,
                            0f,
                            1f
                        ];
                    }
                    else
                    {
                        this.LogOrThrow($"Matrices are expected to have 6 elements, this one has {slices.Length - 1}");
                    }

                    break;
                case "translate":
                    if (slices.Length >= 3)
                    {
                        matrix = SKMatrix.CreateTranslation(this.ReadNumber(raw[slices[1]]), this.ReadNumber(raw[slices[2]]));
                    }
                    else if (slices.Length >= 2)
                    {
                        matrix = SKMatrix.CreateTranslation(this.ReadNumber(raw[slices[1]]), 0f);
                    }

                    break;
                case "scale":
                    if (slices.Length >= 3)
                    {
                        matrix = SKMatrix.CreateScale(this.ReadNumber(raw[slices[1]]), this.ReadNumber(raw[slices[2]]));
                    }
                    else if (slices.Length >= 2)
                    {
                        var scale = this.ReadNumber(raw[slices[1]]);

                        matrix = SKMatrix.CreateScale(scale, scale);
                    }

                    break;
                case "rotate":
                    {
                        var angle = this.ReadNumber(raw[slices[1]]);

                        if (raw[slices.Length] >= 4)
                        {
                            var cx              = this.ReadNumber(raw[slices[2]]);
                            var cy              = this.ReadNumber(raw[slices[3]]);
                            var translation     = SKMatrix.CreateTranslation(cx, cy);
                            var rotation        = SKMatrix.CreateRotationDegrees(angle);
                            var translationBack = SKMatrix.CreateTranslation(0f - cx, 0f - cy);

                            SKMatrix.Concat(ref matrix, translation, rotation);
                            SKMatrix.Concat(ref matrix, matrix, translationBack);
                        }
                        else
                        {
                            matrix = SKMatrix.CreateRotationDegrees(angle);
                        }

                        break;
                    }
                default:
                    this.LogOrThrow($"Can't transform {raw[slices[0]]}");
                    break;
            }

            result = SKMatrix.Concat(result, matrix);
        }

        return result;
    }

    private static ReadOnlySpan<char> ReadHrefString(XElement element) =>
        (element.Attribute("href") ?? element.Attribute(xlink + "href"))?.Value;

    private static SKShaderTileMode ReadSpreadMethod(XElement element) =>
        element.Attribute("spreadMethod")?.Value switch
        {
            "reflect" => SKShaderTileMode.Mirror,
            "repeat"  => SKShaderTileMode.Repeat,
            _ => SKShaderTileMode.Clamp,
        };

    private SKShader? ReadGradient(XElement element)
    {
        var localName = element.Name.LocalName;

        return localName != "linearGradient"
            ? localName == "radialGradient" ? this.ReadRadialGradient(element) : null
            : this.ReadLinearGradient(element);
    }

    private SKShader ReadLinearGradient(XElement element)
    {
        var x                = this.ReadNumber(element.Attribute("x1")!);
        var y                = this.ReadNumber(element.Attribute("y1")!);
        var x2               = this.ReadNumber(element.Attribute("x2")!);
        var y2               = this.ReadNumber(element.Attribute("y2")!);
        var mode             = ReadSpreadMethod(element);
        var sortedDictionary = this.ReadStops(element);

        return SKShader.CreateLinearGradient(new SKPoint(x, y), new SKPoint(x2, y2), sortedDictionary.Values.ToArray(), sortedDictionary.Keys.ToArray(), mode);
    }

    private float ReadNumber(ReadOnlySpan<char> raw)
    {
        if (raw.IsEmpty)
        {
            return 0f;
        }

        var text       = raw.Trim();
        var multiplier = 1f;

        if (unitPattern.IsMatch(text))
        {
            if (text.EndsWith("in", StringComparison.Ordinal))
            {
                multiplier = this.PixelsPerInch;
            }
            else if (text.EndsWith("cm", StringComparison.Ordinal))
            {
                multiplier = this.PixelsPerInch / 2.54f;
            }
            else if (text.EndsWith("mm", StringComparison.Ordinal))
            {
                multiplier = this.PixelsPerInch / 25.4f;
            }
            else if (text.EndsWith("pt", StringComparison.Ordinal))
            {
                multiplier = this.PixelsPerInch / 72f;
            }
            else if (text.EndsWith("pc", StringComparison.Ordinal))
            {
                multiplier = this.PixelsPerInch / 6f;
            }

            text = text[..^2];
        }
        else if (percPattern.IsMatch(text))
        {
            text       = text[..^1];
            multiplier = 0.01f;
        }

        if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            result = 0f;
        }

        return multiplier * result;
    }

    private float ReadNumber(XAttribute attribute) =>
        this.ReadNumber(attribute?.Value);

    private float ReadNumber(Dictionary<string, string> style, string key, float defaultValue)
    {
        var result = defaultValue;

        if (style.TryGetValue(key, out var value))
        {
            result = this.ReadNumber(value);
        }

        return result;
    }

    private float? ReadOptionalNumber(XAttribute attribute) =>
        attribute != null ? this.ReadNumber(attribute.Value) : null;

    private Unit? ReadOptionalUnit(XAttribute attribute) =>
        attribute != null ? this.ReadUnit(attribute.Value) : null;

    private SKShader ReadRadialGradient(XElement e)
    {
        var x                = this.ReadNumber(e.Attribute("cx")!);
        var y                = this.ReadNumber(e.Attribute("cy")!);
        var radius           = this.ReadNumber(e.Attribute("r")!);
        var mode             = ReadSpreadMethod(e);
        var sortedDictionary = this.ReadStops(e);

        return SKShader.CreateRadialGradient(new SKPoint(x, y), radius, sortedDictionary.Values.ToArray(), sortedDictionary.Keys.ToArray(), mode);
    }

    private SKRect ReadRectangle(ReadOnlySpan<char> raw)
    {
        var result = default(SKRect);

        var enumerator = raw.SplitAny([' ', '\t', '\n', '\r']);

        enumerator.MoveNext();
        result.Left = this.ReadNumber(raw[enumerator.Current]);

        enumerator.MoveNext();
        result.Top = this.ReadNumber(raw[enumerator.Current]);

        enumerator.MoveNext();
        result.Right = result.Left + this.ReadNumber(raw[enumerator.Current]);

        enumerator.MoveNext();
        result.Bottom = result.Top + this.ReadNumber(raw[enumerator.Current]);

        return result;
    }

    private SortedDictionary<float, SKColor> ReadStops(XElement element)
    {
        var sortedDictionary = new SortedDictionary<float, SKColor>();

        var @namespace = element.Name.Namespace;

        foreach (var item in element.Elements(@namespace + "stop"))
        {
            var dictionary = ReadStyle(item);
            var key        = this.ReadNumber(dictionary["offset"]);
            var color      = SKColors.Black;
            var alpha      = byte.MaxValue;

            if (dictionary.TryGetValue("stop-color", out var value) && ColorHelper.TryParse(value, out color) && color.Alpha == byte.MaxValue)
            {
                alpha = color.Alpha;
            }

            if (dictionary.TryGetValue("stop-opacity", out var value2))
            {
                alpha = (byte)(this.ReadNumber(value2) * 255f);
            }

            color = color.WithAlpha(alpha);
            sortedDictionary[key] = color;
        }

        return sortedDictionary;
    }

    private Unit ReadUnit(ReadOnlySpan<char> raw)
    {
        if (raw.IsEmpty)
        {
            return Unit.Px(0);
        }

        var text = raw.Trim();

        if (text.EndsWith("%", StringComparison.Ordinal))
        {
            if (!float.TryParse(text[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                result = 0f;
            }

            return Unit.Pc(result);
        }

        return Unit.Px((int)this.ReadNumber(raw));
    }

    private SKPicture Load(XDocument xdoc)
    {
        var root       = xdoc.Root!;
        var @namespace = root.Name.Namespace;

        this.Version     = root.Attribute("version")?.Value;
        this.Title       = root.Element(@namespace + "title")?.Value;
        this.Description = root.Element(@namespace + "desc")?.Value ?? root.Element(@namespace + "description")?.Value;

        var lookup = this.references.GetAlternateLookup<ReadOnlySpan<char>>();

        foreach (var element in root.Descendants())
        {
            var id = (element.Attribute("id")?.Value ?? "").AsSpan().Trim();

            if (!id.IsEmpty)
            {
                lookup[id] = this.ReadDefinition(element);
            }
        }

        var preserveAspectRatioAttribute = root.Attribute("preserveAspectRatio")?.Value;

        var viewBoxAttribute = root.Attribute("viewBox") ?? root.Attribute("viewPort");

        if (viewBoxAttribute != null)
        {
            this.ViewBox = this.ReadRectangle(viewBoxAttribute.Value);
        }

        if (this.CanvasSize.IsEmpty)
        {
            var widthAttribute  = root.Attribute("width")!;
            var heightAttribute = root.Attribute("height")!;

            var width  = this.ReadNumber(widthAttribute);
            var height = this.ReadNumber(heightAttribute);

            var canvasSize = new SKSize(width, height);

            if (widthAttribute == null)
            {
                canvasSize.Width = this.ViewBox.Width;
            }
            else if (widthAttribute.Value.Contains('%'))
            {
                canvasSize.Width *= this.ViewBox.Width;
            }

            if (heightAttribute == null)
            {
                canvasSize.Height = this.ViewBox.Height;
            }
            else if (heightAttribute?.Value.Contains('%') == true)
            {
                canvasSize.Height *= this.ViewBox.Height;
            }

            this.CanvasSize = canvasSize;
        }

        using var pictureRecorder = new SKPictureRecorder();
        using var canvas          = pictureRecorder.BeginRecording(SKRect.Create(this.CanvasSize));

        if (!this.ViewBox.IsEmpty && (this.ViewBox.Width != this.CanvasSize.Width || this.ViewBox.Height != this.CanvasSize.Height))
        {
            if (preserveAspectRatioAttribute == "none")
            {
                canvas.Scale(this.CanvasSize.Width / this.ViewBox.Width, this.CanvasSize.Height / this.ViewBox.Height);
            }
            else
            {
                var scale  = Math.Min(this.CanvasSize.Width / this.ViewBox.Width, this.CanvasSize.Height / this.ViewBox.Height);
                var sKRect = SKRect.Create(this.CanvasSize).AspectFit(this.ViewBox.Size);

                canvas.Translate(sKRect.Left, sKRect.Top);
                canvas.Scale(scale, scale);
            }
        }

        canvas.Translate(0f - this.ViewBox.Left, 0f - this.ViewBox.Top);

        if (!this.ViewBox.IsEmpty)
        {
            canvas.ClipRect(this.ViewBox);
        }

        this.ReadPaints(root, true, CreatePaint(), null, out var fillPaint, out var strokePaint);

        // using var font = new SKFont(SKTypeface.Default);
        using var font = new SKFont(SKTypeface.FromFamilyName("Times New Roman")); // TODO: Remove

        this.LoadElements(root.Elements(), canvas, font, fillPaint, strokePaint);

        fillPaint?.Dispose();
        strokePaint?.Dispose();

        return this.Picture = pictureRecorder.EndRecording();
    }

    private void LoadElements(IEnumerable<XElement> elements, SKCanvas canvas, SKFont font, SKPaint? fillPaint, SKPaint? strokePaint)
    {
        foreach (var element in elements)
        {
            this.ReadElement(element, canvas, font, fillPaint, strokePaint);
        }
    }

    private void LogOrThrow(string message)
    {
        if (this.ThrowOnUnsupportedElement)
        {
            throw new NotSupportedException(message);
        }
    }

    private XElement ReadDefinition(XElement element)
    {
        var union = new XElement(element.Name);

        union.Add(element.Elements());
        union.Add(element.Attributes());

        var reference = this.ReadHref(element);

        if (reference != null)
        {
            union.Add(reference.Elements());

            foreach (var attribute in reference.Attributes())
            {
                if (union.Attribute(attribute.Name) == null)
                {
                    union.Add(attribute);
                }
            }
        }

        return union;
    }

    private XElement? ReadHref(XElement element)
    {
        var text = ReadHrefString(element);

        return text.IsEmpty || !this.references.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(text[1..], out var value) ? null : value;
    }

    private float ReadOpacity(Dictionary<string, string> style) =>
        Math.Min(Math.Max(0f, this.ReadNumber(style, "opacity", 1f)), 1f);

    private Dictionary<string, string> ReadPaints(XElement element, bool isGroup, SKPaint? parentFillPaint, SKPaint? parentStrokePaint, out SKPaint? fillPaint, out SKPaint? strokePaint)
    {
        var style = ReadStyle(element);

        this.ReadPaints(style, isGroup, parentFillPaint, parentStrokePaint, out fillPaint, out strokePaint);

        return style;
    }

    private void ReadPaints(Dictionary<string, string> style, bool isGroup, SKPaint? parentFillPaint, SKPaint? parentStrokePaint, out SKPaint? fillPaint, out SKPaint? strokePaint)
    {
        var opacity     = isGroup ? 1f : this.ReadOpacity(style);

        var strokeValue = GetString(style, "stroke").Trim();
        var fillValue   = GetString(style, "fill").Trim();

        if (fillValue.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            fillPaint = null;
        }
        else
        {
            fillPaint = parentFillPaint?.Clone();

            if (!fillValue.IsEmpty)
            {
                fillPaint ??= CreatePaint();

                if (ColorHelper.TryParse(fillValue, out var fillColor))
                {
                    fillPaint.Color = fillColor.Alpha == byte.MaxValue ? fillColor.WithAlpha(fillPaint.Color.Alpha) : fillColor;
                }
                else
                {
                    var fillUrlHandled = false;

                    var enumerator = fillUrlPattern.EnumerateMatches(fillValue);

                    if (enumerator.MoveNext())
                    {
                        var content = fillValue.Slice(enumerator.Current);

                        var groupEnumerator = groupPattern.EnumerateMatches(content);

                        groupEnumerator.MoveNext();

                        var fillUrlKey = content.Slice(groupEnumerator.Current).Trim();

                        if (this.references.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(fillUrlKey, out var fillDefElement))
                        {
                            var fillShader = this.ReadGradient(fillDefElement);

                            if (fillShader != null)
                            {
                                fillPaint.Shader = fillShader;
                                fillUrlHandled = true;
                            }
                        }
                        else
                        {
                            this.LogOrThrow($"Invalid fill url reference: {fillUrlKey}");
                        }
                    }

                    if (!fillUrlHandled)
                    {
                        this.LogOrThrow($"Unsupported fill: {fillValue}");
                    }
                }
            }

            var fillOpacityValue = GetString(style, "fill-opacity");

            if (!fillOpacityValue.IsEmpty)
            {
                fillPaint ??= CreatePaint();

                fillPaint.Color = fillPaint.Color.WithAlpha((byte)(this.ReadNumber(fillOpacityValue) * 255f));
            }

            fillPaint?.Color = fillPaint.Color.WithAlpha((byte)(fillPaint.Color.Alpha * opacity));
        }

        if (strokeValue.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            strokePaint = null;
        }
        else
        {
            strokePaint = parentStrokePaint?.Clone();

            if (!strokeValue.IsEmpty)
            {
                strokePaint = CreatePaint(true);

                if (ColorHelper.TryParse(strokeValue, out var strokeColor))
                {
                    strokePaint.Color = strokeColor.Alpha == byte.MaxValue ? strokeColor.WithAlpha(strokePaint.Color.Alpha) : strokeColor;
                }
            }

            var strokeDashArray = GetString(style, "stroke-dasharray");

            if (!strokeDashArray.IsEmpty)
            {
                if ("none".Equals(strokeDashArray, StringComparison.OrdinalIgnoreCase))
                {
                    strokePaint?.PathEffect = null;
                }
                else
                {
                    strokePaint ??= CreatePaint(true);

                    var dashArrayNumbers = new List<float>(4);

                    foreach (var range in strokeDashArray.SplitAny([' ', ',']))
                    {
                        dashArrayNumbers.Add(this.ReadNumber(strokeDashArray[range]));
                    }

                    if (strokeDashArray.Length % 2 == 1)
                    {
                        dashArrayNumbers.AddRange(dashArrayNumbers);
                    }

                    var dashOffset = this.ReadNumber(style, "stroke-dashoffset", 0f);

                    strokePaint.PathEffect = SKPathEffect.CreateDash([.. dashArrayNumbers], dashOffset);
                }
            }

            var strokeWidthValue = GetString(style, "stroke-width");

            if (!strokeWidthValue.IsEmpty)
            {
                strokePaint ??= CreatePaint(true);

                strokePaint.StrokeWidth = this.ReadNumber(strokeWidthValue);
            }

            var strokeOpacityValue = GetString(style, "stroke-opacity");

            if (!strokeOpacityValue.IsEmpty)
            {
                strokePaint ??= CreatePaint(true);

                strokePaint.Color = strokePaint.Color.WithAlpha((byte)(this.ReadNumber(strokeOpacityValue) * 255f));
            }

            strokePaint?.Color = strokePaint.Color.WithAlpha((byte)(strokePaint.Color.Alpha * opacity));
        }
    }

    public SKPicture Load(string filepath)
    {
        using var stream = File.OpenRead(filepath);
        return this.Load(stream);
    }

    public SKPicture Load(Stream stream)
    {
        using var reader = XmlReader.Create(stream, this.xmlReaderSettings, CreateSvgXmlContext());
        return this.Load(reader);
    }

    public SKPicture Load(XmlReader reader) =>
        this.Load(XDocument.Load(reader));
}
