using Age.Core.Extensions;
using Age.Styling;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Age.Elements.Layouts;

internal abstract partial class StyledLayout(Element target) : Layout
{
    public event Action<StyleProperty>? StyleChanged;

    private static readonly Style     empty     = new();
    private static readonly StylePool stylePool = new();

    private Style? computedStyle;

    [MemberNotNullWhen(true, nameof(computedStyle), nameof(StyleSheet))]
    private bool NeedsCompute => this.StyleSheet != null;

    private ElementState States
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (this.NeedsCompute)
                {
                    this.ComputeStyle(this.ComputedStyle.Data);
                }
            }
        }
    }

    public Style ComputedStyle => this.computedStyle ?? this.UserStyle ?? empty;

    public override Element Target => target;

    public StyleSheet? StyleSheet
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                var previous = this.ComputedStyle.Data;
                this.HandleComputedStyleAllocation(value != null);
                this.ComputeStyle(previous);
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

                var previous = this.ComputedStyle.Data;

                field = value;

                this.ComputeStyle(previous);
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
        if (!target.IsConnected)
        {
            return;
        }

        if (!this.NeedsCompute)
        {
            this.CompareAndInvoke(this.ComputedStyle.Data, previous);

            return;
        }

        if (this.StyleSheet.Base != null)
        {
            this.computedStyle.Copy(this.StyleSheet.Base);
        }
        else
        {
            this.computedStyle.Clear();
        }

        if (this.UserStyle != null)
        {
            this.computedStyle.Merge(this.UserStyle);
        }

        merge(ElementState.Focus,    this.StyleSheet.Focus);
        merge(ElementState.Hovered,  this.StyleSheet.Hovered);
        merge(ElementState.Disabled, this.StyleSheet.Disabled);
        merge(ElementState.Enabled,  this.StyleSheet.Enabled);
        merge(ElementState.Checked,  this.StyleSheet.Checked);
        merge(ElementState.Active,   this.StyleSheet.Active);

        this.CompareAndInvoke(this.computedStyle.Data, previous);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void merge(ElementState states, Style? style)
        {
            if (this.States.HasFlags(states) && style != null)
            {
                this.computedStyle.Merge(style);
            }
        }
    }

    private void HandleComputedStyleAllocation(bool allocating)
    {
        if (allocating)
        {
            this.computedStyle ??= stylePool.Get();
        }
        else
        {
            stylePool.Return(this.computedStyle!);
            this.computedStyle = null;
        }
    }

    private void InvokeStyleChanged(StyleProperty property)
    {
        this.OnStyleChanged(property);
        StyleChanged?.Invoke(property);
    }

    private void OnPropertyChanged(StyleProperty property)
    {
        this.computedStyle?.Copy(this.UserStyle!, property);
        this.InvokeStyleChanged(property);
    }

    protected abstract void OnStyleChanged(StyleProperty property);

    public void AddState(ElementState state) =>
        this.States |= state;

    public void ComputeStyle() =>
        this.ComputeStyle(default);

    public void RemoveState(ElementState state) =>
        this.States &= ~state;

    public override string ToString() =>
        $"{{ Target: {target} }}";
}
