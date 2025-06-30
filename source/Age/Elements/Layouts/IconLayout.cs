using Age.Commands;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Resources;
using Age.Storage;
using Age.Styling;
using SkiaSharp;

using static Age.Shaders.CanvasShader;

namespace Age.Elements.Layouts;

internal sealed class IconLayout : StyledLayout
{
    private const string MATERIAL_ICONS_OUTLINED = nameof(MATERIAL_ICONS_OUTLINED);

    private static readonly Dictionary<string, string> defaultFontFaces = new()
    {
        [MATERIAL_ICONS_OUTLINED] = Path.Join(AppContext.BaseDirectory, "Assets", "Fonts", "MaterialIconsOutlined-Regular.otf")
    };

    private Dictionary<string, int>? codepoints;
    private SKFont                   font;
    private bool                     isDirty;

    public IconLayout(Icon target)
    {
        this.Target = target;

        var command = CommandPool.RectCommand.Get();

        this.Target.Commands.Add(command);

        this.font       = TextStorage.Singleton.GetFont(MATERIAL_ICONS_OUTLINED, this.FontSize, (int)FontWeight.Normal, defaultFontFaces);
        this.codepoints = TextStorage.Singleton.GetCodepoints(MATERIAL_ICONS_OUTLINED, defaultFontFaces);
    }

    public override StencilLayer? StencilLayer
    {
        get => base.StencilLayer;
        set
        {
            if (base.StencilLayer != value)
            {
                base.StencilLayer = value;

                foreach (var command in this.Target.Commands)
                {
                    command.StencilLayer = value;
                }
            }
        }
    }

    public override bool IsParentDependent { get; }

    public override Icon Target { get; }

    public string? IconName
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.isDirty = true;
                this.RequestUpdate(true);
            }
        }
    }

    private void Draw()
    {
        if (this.font == null)
        {
            throw new InvalidOperationException();
        }

        var command = this.GetRectCommand();

        if (this.codepoints?.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(this.IconName, out var codepoint) is null or false)
        {
            command.Color           = default;
            command.Flags           = default;
            command.TextureMap      = TextureMap.Default;
            command.ObjectId        = default;
            command.PipelineVariant = default;
            command.Size            = default;
            command.StencilLayer    = default;

            return;
        }

        var style = this.Target.ComposedParentElement!.Layout.ComputedStyle;

        var glyphs = this.font.Typeface.GetGlyphs([codepoint]);
        var atlas  = TextStorage.Singleton.GetAtlas(this.font.Typeface.FamilyName, (uint)this.font.Size);

        Span<SKRect> glyphsBounds = stackalloc SKRect[1];
        Span<float>  glyphsWidths = stackalloc float[1];

        this.font.GetGlyphWidths(glyphs, glyphsWidths, glyphsBounds);

        ref var bounds = ref glyphsBounds[0];

        var glyph = TextStorage.Singleton.DrawGlyph(this.font, atlas, [(char)codepoint], bounds);

        command.Color           = style.Color ?? new();
        command.Flags           = Flags.GrayscaleTexture | Flags.MultiplyColor;
        command.ObjectId        = default;
        command.PipelineVariant = PipelineVariant.Color;
        command.Size            = new Size<float>(bounds.Width, bounds.Height);
        command.StencilLayer    = this.StencilLayer;
        command.TextureMap      = glyph;
        command.Transform       = Transform2D.CreateTranslated(float.Round(bounds.Left), float.Round(-this.BaseLine - bounds.Top));

        atlas.Update();

        this.Boundings = command.Size.Cast<uint>();
    }

    private RectCommand GetRectCommand() =>
        (RectCommand)this.Target.Commands[0];

    protected override void OnStyleChanged(StyleProperty property)
    {
        if (property.HasAnyFlag(StyleProperty.FontFamily | StyleProperty.FontFeature | StyleProperty.FontWeight))
        {
            var style = this.ComputedStyle;

            var fontFamily = string.Intern(style.FontFamily ?? MATERIAL_ICONS_OUTLINED);
            var fontWeight = (int)(style.FontWeight ?? FontWeight.Normal);
            var fontSize   = this.FontSize;

            var fontFaces = this.StyleSheet?.FontFaces ?? defaultFontFaces;

            if (this.font?.Size != fontSize || this.font.Typeface.FamilyName != fontFamily || this.font.Typeface.FontWeight != fontWeight)
            {
                this.font = TextStorage.Singleton.GetFont(fontFamily, fontSize, fontWeight, fontFaces);
                this.font.GetFontMetrics(out var metrics);

                this.LineHeight = (uint)float.Round(-metrics.Ascent + metrics.Descent);
                this.BaseLine   = (int)float.Round(-metrics.Ascent);
            }

            this.codepoints = TextStorage.Singleton.GetCodepoints(fontFamily, fontFaces);

            this.isDirty = true;
            this.RequestUpdate(true);
        }
    }

    protected override void OnDisposed()
    {
        CommandPool.RectCommand.Return(this.GetRectCommand());

        this.Target.Commands.Clear();
    }

    public void HandleTargetIndexed()
    {
        this.UpdateDirtyLayout();

        var command = this.GetRectCommand();

        command.ObjectId = (ulong)(this.Target.Index + 1);
    }

    public override void Update()
    {
        if (this.isDirty)
        {
            this.Draw();
        }

        this.isDirty = false;
    }
}
