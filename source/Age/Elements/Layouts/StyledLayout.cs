using Age.Core.Extensions;
using Age.Styling;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Age.Elements.Layouts;

internal abstract partial class StyledLayout(Element target) : Layout
{
    public event Action<StyleProperty>? StyleChanged;

    private ElementState States
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.ComputeStyle();
            }
        }
    }

    [field: AllowNull]
    public Style ComputedStyle { get; } = new();

    public override Element Target => target;

    public StyleSheet? StyleSheet
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.ComputeStyle();
            }
        }
    }

    public Style? UserStyle
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

                this.ComputeStyle();
            }
        }
    }

    private void InvokeStyleChanged(StyleProperty property)
    {
        this.OnStyleChanged(property);
        StyleChanged?.Invoke(property);
    }

    private void OnPropertyChanged(StyleProperty property)
    {
        this.ComputedStyle.Copy(this.UserStyle!, property);
        this.InvokeStyleChanged(property);
    }

    protected abstract void OnStyleChanged(StyleProperty property);

    public void AddState(ElementState state) =>
        this.States |= state;

    public void ComputeStyle()
    {
        if (!target.IsConnected)
        {
            return;
        }

        var previous = this.ComputedStyle.Data;

        if (this.StyleSheet?.Base != null)
        {
            this.ComputedStyle.Copy(this.StyleSheet.Base);
        }
        else
        {
            this.ComputedStyle.Clear();
        }

        if (this.UserStyle != null)
        {
            this.ComputedStyle.Merge(this.UserStyle);
        }

        merge(ElementState.Focus,    this.StyleSheet?.Focus);
        merge(ElementState.Hovered,  this.StyleSheet?.Hovered);
        merge(ElementState.Disabled, this.StyleSheet?.Disabled);
        merge(ElementState.Enabled,  this.StyleSheet?.Enabled);
        merge(ElementState.Checked,  this.StyleSheet?.Checked);
        merge(ElementState.Active,   this.StyleSheet?.Active);

        var changes = StyleData.Diff(this.ComputedStyle.Data, previous);

        if (changes != default)
        {
            this.InvokeStyleChanged(changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void merge(ElementState states, Style? style)
        {
            if (this.States.HasFlags(states) && style != null)
            {
                this.ComputedStyle.Merge(style);
            }
        }
    }

    public void RemoveState(ElementState state) =>
        this.States &= ~state;

    public override string ToString() =>
        $"{{ Target: {target} }}";
}
