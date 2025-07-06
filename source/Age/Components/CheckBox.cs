using Age.Elements;
using Age.Themes;

namespace Age.Components;

public class CheckBox : Element
{
    private const string CHECKED       = "check_box";
    private const string UNCHECKED     = "check_box_outline_blank";
    private const string INDETERMINATE = "indeterminate_check_box";

    public event Action? Changed;

    public override string NodeName => nameof(CheckBox);
    private readonly Icon icon = new(UNCHECKED);

    public bool Checked
    {
        get;
        set
        {
            if (field != value)
            {
                this.icon.IconName = value ? CHECKED : UNCHECKED;

                field = value;

                this.Changed?.Invoke();
            }
        }
    }

    public bool Indeterminate
    {
        get;
        set
        {
            if (field != value)
            {
                this.icon.IconName =
                    value
                    ? INDETERMINATE
                    : this.Checked
                        ? CHECKED
                        : UNCHECKED;

                field = value;

                this.Changed?.Invoke();
            }
        }
    }

    public CheckBox()
    {
        this.Flags       = Scene.NodeFlags.Immutable;
        this.IsFocusable = true;
        this.StyleSheet  = Theme.Current.CheckBox.Default;

        this.AttachShadowTree(true);

        this.ShadowTree.AppendChild(this.icon);

        this.icon.Clicked += this.OnClick;
    }

    private void OnClick(in MouseEvent mouseEvent) =>
        this.Checked = !this.Checked;
}
