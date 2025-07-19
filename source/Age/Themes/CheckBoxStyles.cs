using Age.Numerics;
using Age.Styling;

namespace Age.Themes;

public record CheckBoxStyles
{
    private const string MATERIAL_ICONS_OUTLINED = nameof(MATERIAL_ICONS_OUTLINED);

    public static CheckBoxStyles GetDarkVariant() =>
        new()
        {
            Default = new()
            {
                Base = new()
                {
                    Color           = Color.Green,
                    BackgroundColor = Color.Green.WithAlpha(0),
                },
                Focused = new()
                {
                    BackgroundColor = Color.Green.WithAlpha(0.15f),
                },
                Hovered = new()
                {
                    Color           = Color.Green + (Color.White * 0.6f),
                    BackgroundColor = Color.Green.WithAlpha(0.2f),
                },
                Active = new()
                {
                    Color           = Color.Green + (Color.White * 0.9f),
                    BackgroundColor = Color.Green.WithAlpha(0.5f),
                },
            }
        };

    public required StyleSheet Default { get; init; }
}
