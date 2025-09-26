using Age.Core.Extensions;
using Age.Styling;
using System.Runtime.CompilerServices;

namespace Age.Elements;

public abstract partial class Element
{
    private static readonly StylePool stylePool = new();

    internal event Action<StyleProperty>? StyleChanged;

    private Style? UserStyle
    {
        get;
        set
        {
            if (field != value)
            {
                if (field != null)
                {
                    field.PropertyChanged -= this.OnPropertyChanged;
                }

                if (value != null)
                {
                    value.PropertyChanged += this.OnPropertyChanged;
                }

                field = value;

                this.ComputeStyle(this.ComputedStyle.Data);
            }
        }
    }

    public Style ComputedStyle { get; } = stylePool.Get();

    public Style Style
    {
        get => this.UserStyle ??= new();
        set => this.UserStyle = value;
    }

    public StyleSheet? StyleSheet
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.ComputeStyle(this.ComputedStyle.Data);
            }
        }
    }

    private void CompareAndInvoke(in StyleData left, in StyleData right)
    {
        var changes = StyleData.Diff(left, right);

        if (changes != default)
        {
            this.InvokeStyleChanged(changes);
        }
    }

    private void ComputeStyle(in StyleData previous)
    {
        if (!this.IsConnected)
        {
            return;
        }

        var inheritedProperties = this.GetInheritedProperties();

        this.ComputedStyle.Copy(inheritedProperties);

        if (this.StyleSheet?.Base != null)
        {
            this.ComputedStyle.Merge(this.StyleSheet.Base);
        }

        if (this.UserStyle != null)
        {
            this.ComputedStyle.Merge(this.UserStyle);
        }

        if (this.StyleSheet != null)
        {
            merge(State.Focused,  this.StyleSheet.Focused);
            merge(State.Hovered,  this.StyleSheet.Hovered);
            merge(State.Disabled, this.StyleSheet.Disabled);
            merge(State.Enabled,  this.StyleSheet.Enabled);
            merge(State.Checked,  this.StyleSheet.Checked);
            merge(State.Active,   this.StyleSheet.Active);
        }

        this.CompareAndInvoke(this.ComputedStyle.Data, previous);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void merge(State states, Style? style)
        {
            if (this.ActiveStates.HasFlags(states) && style != null)
            {
                this.ComputedStyle.Merge(style);
            }
        }
    }

    private void InvokeStyleChanged(StyleProperty property)
    {
        this.OnStyleChanged(property);

        StyleChanged?.Invoke(property);
    }

    private StyleData GetInheritedProperties() =>
        GetStyleSource(this.Parent)?.ComputedStyle is Style parentStyle
            ? new StyleData
            {
                Color         = parentStyle.Color,
                FontFamily    = parentStyle.FontFamily,
                FontSize      = parentStyle.FontSize,
                FontWeight    = parentStyle.FontWeight,
                TextSelection = parentStyle.TextSelection
            }
            : default;

    private void OnParentStyleChanged(StyleProperty _)
    {
        var inheritedProperties = this.GetInheritedProperties();

        this.ComputedStyle.Merge(inheritedProperties);
    }

    private void OnPropertyChanged(StyleProperty property)
    {
        this.ComputedStyle?.Copy(this.UserStyle!, property);
        this.InvokeStyleChanged(property);
    }
}
