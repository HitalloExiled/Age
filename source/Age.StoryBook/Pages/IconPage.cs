using Age.Components;

namespace Age.StoryBook.Pages;

public class IconPage : Page
{
    public override string NodeName => nameof(IconPage);
    public override string Title    => nameof(Icon);

    public IconPage() =>
        this.Children =
        [
            new Icon("search"),
            new Icon("home"),
            new Icon("close"),
        ];
}
