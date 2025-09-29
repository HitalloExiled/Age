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

    public override string NodeName => nameof(Icon);

    public Icon(string? iconName = null, ushort? fontSize = null, Color? color = null)
    {
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

        this.StyleChanged += this.OnHostStyleChanged;

        this.Seal();
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

    protected void OnHostStyleChanged(StyleProperty property)
    {
        if (property.HasFlags(StyleProperty.FontFamily))
        {
            this.codepoints = this.ComputedStyle.FontFamily is string fontFamily
                ? TextStorage.Singleton.GetCodepoints(fontFamily, this.StyleSheet?.FontFaces)
                : null;

            this.SetCodepoint(this.IconName);
        }
    }
}
