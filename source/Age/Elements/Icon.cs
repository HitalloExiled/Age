using Age.Core.Extensions;
using Age.Numerics;
using Age.Scene;
using Age.Storage;
using Age.Styling;

namespace Age.Elements;

public class Icon : Element
{
    public override string NodeName => nameof(Icon);

    private const string MATERIAL_ICONS_OUTLINED = nameof(MATERIAL_ICONS_OUTLINED);

    private readonly Text text = new();

    private Dictionary<string, int>? codepoints;

    public string? IconName
    {
        get;
        set
        {
            if (field != value)
            {
                this.SetCodepoint(value);

                field = value;
            }
        }
    }

    public Icon(string? iconName = null, ushort? fontSize = null, Color? color = null)
    {
        this.Flags = NodeFlags.Immutable;

        this.IconName = iconName;

        if (fontSize.HasValue)
        {
            this.Style.FontSize = fontSize;
        }

        if (color.HasValue)
        {
            this.Style.Color = color;
        }

        this.StyleSheet = new()
        {
            Base = new()
            {
                FontFamily    = MATERIAL_ICONS_OUTLINED,
                FontSize      = 24,
                TextSelection = false,
            },
            FontFaces =
            {
                [MATERIAL_ICONS_OUTLINED] = Path.Join(AppContext.BaseDirectory, "Assets", "Fonts", "MaterialIconsOutlined-Regular.otf")
            }
        };

        this.AttachShadowTree();
        this.ShadowTree.AppendChild(this.text);

        this.Layout.StyleChanged += this.OnStyleChanged;
    }

    private void SetCodepoint(string? iconName)
    {
        if (iconName != null && this.codepoints?.TryGetValue(iconName, out var codepoint) == true)
        {
            this.text.Buffer.Set([(char)codepoint]);
        }
        else
        {
            this.text.Buffer.Clear();
        }
    }

    private void OnStyleChanged(StyleProperty property)
    {
        if (property.HasFlags(StyleProperty.FontFamily))
        {
            this.codepoints = TextStorage.Singleton.GetCodepoints(this.Layout.ComputedStyle.FontFamily ?? MATERIAL_ICONS_OUTLINED, this.StyleSheet?.FontFaces);

            this.SetCodepoint(this.IconName);
        }
    }
}
