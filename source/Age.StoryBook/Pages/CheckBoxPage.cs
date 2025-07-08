using Age.Components;

namespace Age.StoryBook.Pages;

public class CheckBoxPage : Page
{
    public override string NodeName => nameof(CheckBoxPage);
    public override string Title    => nameof(CheckBox);

    public CheckBoxPage() =>
        this.Children =
        [
            new CheckBox { Checked       = true },
            new CheckBox { Checked       = false },
            new CheckBox { Indeterminate = true },
        ];
}
