using Age.Core.Extensions;
using Age.Elements;
using Age.Numerics;
using Age.Scene;
using Age.Storage;
using Age.Styling;
using Age.Themes;

namespace Age.Components;

public class Icon : Element
{
    public override string NodeName => nameof(Icon);

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

        this.StyleSheet = Theme.Current.Icon.Default;

        this.AttachShadowTree(true);
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
            if (this.Layout.ComputedStyle.FontFamily is string fontFamily)
            {
                this.codepoints = TextStorage.Singleton.GetCodepoints(fontFamily, this.StyleSheet?.FontFaces);
            }
            else
            {
                this.codepoints = null;
            }

            this.SetCodepoint(this.IconName);
        }
    }
}
