using Age.Components;
using Age.Elements;

namespace Age.StoryBook.Pages;

public class CheckBoxPage : Page
{
    public override string NodeName => nameof(CheckBoxPage);
    public override string Title    => nameof(CheckBox);

    public CheckBoxPage() =>
        this.Children =
        [
            new Text("Unchecked: "),
            new CheckBox { Checked = false, Readonly = true },
            new Text("Checked: "),
            new CheckBox { Checked = true, Readonly = true },
            new Text("Indeterminate: "),
            new CheckBox { State = CheckBoxState.Indeterminate, Readonly = true },
            new Text("TriState: "),
            new CheckBox { TriState = true },
        ];
}
