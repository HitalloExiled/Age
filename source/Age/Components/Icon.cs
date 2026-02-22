using System.Runtime.CompilerServices;
using Age.Core.Extensions;
using Age.Elements;
using Age.Numerics;
using Age.Storage;
using Age.Styling;
using Age.Themes;

namespace Age.Components;

public class Icon : Element
{
    private Dictionary<string, int>? codepoints;

    private Text Text => Unsafe.As<Text>(this.ShadowRoot)!;

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
        this.AttachShadowRoot(new Text());

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

        this.StyleChanged += this.OnHostStyleChanged;

        this.Seal();
    }

    private void SetCodepoint(string? iconName)
    {
        if (iconName != null && this.codepoints?.TryGetValue(iconName, out var codepoint) == true)
        {
            this.Text.Buffer.Set([(char)codepoint]);
        }
        else
        {
            this.Text.Buffer.Clear();
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
