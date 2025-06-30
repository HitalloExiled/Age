using Age.Commands;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Storage;
using Age.Styling;
using SkiaSharp;

using static Age.Shaders.CanvasShader;
using Age.Core.Collections;
using Age.Resources;

namespace Age.Elements.Layouts;

internal sealed class IconLayout : Layout
{
    private Dictionary<string, int>?  codepoints;
    private SKFont?                   font;
    private bool                      isDirty;

    public IconLayout(Icon target)
    {
        this.Target = target;

        var command = CommandPool.RectCommand.Get();

        this.Target.Commands.Add(command);
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
        var size  = new Size<float>(bounds.Width, bounds.Height);

        command.Color           = style.Color ?? new();
        command.Flags           = Flags.GrayscaleTexture | Flags.MultiplyColor;
        command.TextureMap      = glyph;
        command.ObjectId        = default;
        command.PipelineVariant = PipelineVariant.Color;
        command.Size            = size;
        command.StencilLayer    = this.StencilLayer;

        atlas.Update();

        this.Boundings = size.Cast<uint>();
    }

    private RectCommand GetRectCommand() =>
        (RectCommand)this.Target.Commands[0];

    private void OnParentStyleChanged(StyleProperty property)
    {
        if (property.HasAnyFlag(StyleProperty.FontFamily | StyleProperty.FontFeature | StyleProperty.FontWeight))
        {
            var style = this.Parent!.ComputedStyle;

            var fontFamily = string.Intern(style.FontFamily ?? "Segoi UI");
            var fontWeight = (int)(style.FontWeight ?? FontWeight.Normal);
            var fontSize   = this.Parent.FontSize;

            if (this.font?.Size != fontSize || this.font.Typeface.FamilyName != fontFamily || this.font.Typeface.FontWeight != fontWeight)
            {
                this.font = TextStorage.Singleton.GetFont(fontFamily, fontSize, fontWeight, this.Parent.StyleSheet?.FontFaces);
                this.font.GetFontMetrics(out var metrics);

                this.LineHeight  = (uint)float.Round(-metrics.Ascent + metrics.Descent);
                this.BaseLine    = (int)float.Round(-metrics.Ascent);
            }

            this.codepoints = TextStorage.Singleton.GetCodepoints(fontFamily, this.Parent.StyleSheet?.FontFaces);

            this.isDirty = true;
            this.RequestUpdate(true);
        }
    }

    protected override void OnDisposed()
    {
        CommandPool.RectCommand.Return(this.GetRectCommand());

        this.Target.Commands.Clear();
    }

    public void HandleTargetAdopted(Element parentElement)
    {
        parentElement.Layout.StyleChanged += this.OnParentStyleChanged;

        this.OnParentStyleChanged(StyleProperty.All);
    }

    public void HandleTargetIndexed()
    {
        this.UpdateDirtyLayout();

        var command = this.GetRectCommand();

        command.ObjectId = (ulong)(this.Target.Index + 1);
    }

    public void HandleTargetRemoved(Element parentElement) =>
        parentElement.Layout.StyleChanged -= this.OnParentStyleChanged;

    public override void Update()
    {
        if (this.isDirty)
        {
            this.Draw();
        }

        this.isDirty = false;
    }
}
