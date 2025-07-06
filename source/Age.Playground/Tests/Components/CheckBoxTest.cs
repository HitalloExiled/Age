using Age.Components;
using Age.Elements;

namespace Age.Playground.Tests.Components;

public static class CheckBoxTest
{
    public static void Setup(Canvas canvas) =>
        canvas.Children =
        [
            new CheckBox { Checked = true },
            new CheckBox { Checked = false },
            new CheckBox { Indeterminate = true },
        ];
}
