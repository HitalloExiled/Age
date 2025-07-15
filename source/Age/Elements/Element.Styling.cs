using System.Runtime.CompilerServices;
using Age.Core.Extensions;
using Age.Scene;
using Age.Styling;

namespace Age.Elements;

public abstract partial class Element
{
    internal event Action<StyleProperty>? StyleChanged;

    private static readonly StylePool stylePool = new();

    private ElementState States
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (this.StyleSheet != null)
                {
                    this.ComputeStyle(this.ComputedStyle.Data);
                }
            }
        }
    }

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

                var previous = this.ComputedStyle.Data;

                field = value;

                this.ComputeStyle(previous);
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
        if (!IsConnected)
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
            merge(ElementState.Focus,    this.StyleSheet.Focus);
            merge(ElementState.Hovered,  this.StyleSheet.Hovered);
            merge(ElementState.Disabled, this.StyleSheet.Disabled);
            merge(ElementState.Enabled,  this.StyleSheet.Enabled);
            merge(ElementState.Checked,  this.StyleSheet.Checked);
            merge(ElementState.Active,   this.StyleSheet.Active);
        }

        this.CompareAndInvoke(this.ComputedStyle.Data, previous);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void merge(ElementState states, Style? style)
        {
            if (this.States.HasFlags(states) && style != null)
            {
                this.ComputedStyle.Merge(style);
            }
        }
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

    private void InvokeStyleChanged(StyleProperty property)
    {
        this.OnStyleChanged(property);
        StyleChanged?.Invoke(property);
    }

    private void OnParentStyleChanged(StyleProperty property)
    {
        var inheritedProperties = this.GetInheritedProperties();

        this.ComputedStyle.Merge(inheritedProperties);
    }

    private void OnPropertyChanged(StyleProperty property)
    {
        this.ComputedStyle?.Copy(this.UserStyle!, property);
        this.InvokeStyleChanged(property);
    }

    protected override void OnRemoved(Node parent)
    {
        base.OnRemoved(parent);

        GetStyleSource(parent)?.StyleChanged -= this.OnParentStyleChanged;
    }

    private protected void AddState(ElementState state) =>
        this.States |= state;

    private protected void RemoveState(ElementState state) =>
        this.States &= ~state;
}
