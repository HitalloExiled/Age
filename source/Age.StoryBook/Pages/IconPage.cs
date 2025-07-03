using Age.Elements;
using Age.Numerics;

namespace Age.StoryBook.Pages;

public class IconPage : Page
{
    public override string NodeName => nameof(IconPage);
    public override string Title    => "Icon";

    public IconPage() =>
        this.Children =
        [
            new Icon("search", color: Color.White),
            new Icon("home",   color: Color.White),
            new Icon("close",  color: Color.White),
        ];
}
