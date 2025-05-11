using System.Diagnostics;
using System.Text;
using SkiaSharp;
using ThirdParty.Skia.Svg;

namespace ThirdParty.Tests.Skia.Svg;

public class SkSvgTest
{
    private static readonly string workingDirectory = Debugger.IsAttached
        ? Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "../../.."))
        : Directory.GetCurrentDirectory();

    private static readonly string debugDirectory = Path.Join(workingDirectory, "Skia", "Svg", ".debug");
    private static readonly string iconsFolder = Path.Join(workingDirectory, "Skia", "Svg", "Icons");

    private static string GetAssetPath(string name) =>
        Path.Join(iconsFolder, $"{name}.svg");

    private static void Write(SkSvg svg, string filename)
    {
        using var bitmap = new SKBitmap((int)svg.CanvasSize.Width, (int)svg.CanvasSize.Height);
        using var canvas = new SKCanvas(bitmap);

        canvas.DrawPicture(svg.Picture);
        canvas.Flush();
        canvas.Save();

        using var image = SKImage.FromBitmap(bitmap);

        Directory.CreateDirectory(debugDirectory);

        using var stream = File.OpenWrite(Path.IsPathFullyQualified(filename) ? filename : Path.Join(debugDirectory, $"{filename}.png"));

        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
    }

    [Fact]
    public void Load()
    {
        var name = "check_box";
        var filename = GetAssetPath(name);

        var svg = new SkSvg();

        svg.Load(filename);

        Write(svg, name);
    }

    [Fact]
    public void LoadAll()
    {
        var iconsDirectory = new DirectoryInfo(iconsFolder);

        foreach (var file in iconsDirectory.GetFiles())
        {
            var svg = new SkSvg();

            svg.Load(file.FullName);

            Write(svg, file.Name);
        }
    }

    [Fact]
    public void LoadText()
    {
        var source =
        """
        <svg height="40" width="250" xmlns="http://www.w3.org/2000/svg">
            <text x="5" y="30" fill="red" font-size="35">I Love
                <tspan fill="none" stroke="green">SVG</tspan>!
            </text>
        </svg>
        """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(source));

        var svg = new SkSvg();

        svg.Load(stream);

        Write(svg, "text");
    }
}
