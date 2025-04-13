
using Age.Core.Extensions;
using Age.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Age.Styling;

internal partial class StyledStateManager(Element target)
{
    public event Action<StyleProperty>? Changed;

    private State States
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.Update();
            }
        }
    }

    [field: AllowNull]
    public Style Style => field ??= new();

    public StyledStates? Styles
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                this.Update();
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
                    field.Changed -= this.OnStyleChanged;
                }

                if (value != null)
                {
                    value.Changed += this.OnStyleChanged;
                }

                field = value;

                this.Update();
            }
        }
    }

    private void OnStyleChanged(StyleProperty property)
    {
        this.Style.Copy(this.UserStyle!, property);
        this.Changed?.Invoke(property);
    }



    public void AddState(State state) =>
        this.States |= state;

    public void RemoveState(State state) =>
        this.States &= ~state;

    public void Update()
    {
        if (!target.IsConnected)
        {
            return;
        }

        var previous = this.Style.Data;

        if (this.Styles?.Base != null)
        {
            this.Style.Copy(this.Styles.Base);
        }
        else
        {
            this.Style.Clear();
        }

        if (this.UserStyle != null)
        {
            this.Style.Merge(this.UserStyle);
        }

        merge(State.Focus,    this.Styles?.Focus);
        merge(State.Hovered,  this.Styles?.Hovered);
        merge(State.Disabled, this.Styles?.Disabled);
        merge(State.Enabled,  this.Styles?.Enabled);
        merge(State.Checked,  this.Styles?.Checked);
        merge(State.Active,   this.Styles?.Active);

        var changes = StyleData.Diff(this.Style.Data, previous);

        if (changes != default)
        {
            this.Changed?.Invoke(changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void merge(State states, Style? style)
        {
            if (this.States.HasFlags(states) && style != null)
            {
                this.Style.Merge(style);
            }
        }
    }

    public override string ToString() =>
        $"{{ Target: {target} }}";
}
