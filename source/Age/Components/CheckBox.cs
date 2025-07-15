using Age.Elements;
using Age.Themes;

namespace Age.Components;

public enum CheckBoxState : byte
{
    Unchecked,
    Checked,
    Indeterminate,
}

public class CheckBox : Element
{
    private const string CHECKED       = "check_box";
    private const string INDETERMINATE = "indeterminate_check_box";
    private const string UNCHECKED     = "check_box_outline_blank";

    public event Action? Changed;

    private readonly Icon icon = new(UNCHECKED);

    public override string NodeName => nameof(CheckBox);

    public bool Checked
    {
        get => this.State == CheckBoxState.Checked;
        set => this.State = value ? CheckBoxState.Checked : CheckBoxState.Unchecked;
    }

    public bool Readonly { get; set; }

    public CheckBoxState State
    {
        get;
        set
        {
            if (field != value)
            {
                this.icon.IconName = value switch
                {
                    CheckBoxState.Unchecked     => UNCHECKED,
                    CheckBoxState.Checked       => CHECKED,
                    CheckBoxState.Indeterminate => INDETERMINATE,
                    _ => throw new NotSupportedException(),
                };

                field = value;

                this.Changed?.Invoke();
            }
        }
    }

    public bool TriState { get; set; }

    public CheckBox()
    {
        this.NodeFlags       = Scene.NodeFlags.Immutable;
        this.IsFocusable = true;
        this.StyleSheet  = Theme.Current.CheckBox.Default;

        this.AttachShadowTree(true);

        this.ShadowTree.AppendChild(this.icon);

        this.icon.Clicked += this.OnClick;
    }

    private void OnClick(in MouseEvent mouseEvent)
    {
        if (this.Readonly)
        {
            return;
        }

        if (!this.TriState)
        {
            this.Checked = !this.Checked;
        }
        else
        {
            this.State = this.State switch
            {
                CheckBoxState.Unchecked     => CheckBoxState.Checked,
                CheckBoxState.Checked       => CheckBoxState.Indeterminate,
                CheckBoxState.Indeterminate => CheckBoxState.Unchecked,
                _ => throw new NotSupportedException(),
            };
        }
    }
}
