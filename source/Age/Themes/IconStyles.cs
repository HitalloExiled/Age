using Age.Numerics;
using Age.Styling;

namespace Age.Themes;

public record IconStyles
{
    private const string MATERIAL_ICONS_OUTLINED = nameof(MATERIAL_ICONS_OUTLINED);

    public static IconStyles GetDarkVariant() =>
        new()
        {
            Default = new()
            {
                Base = new()
                {
                    Color         = Color.White,
                    FontFamily    = MATERIAL_ICONS_OUTLINED,
                    FontSize      = 24,
                    TextSelection = false,
                },
                FontFaces =
                {
                    [MATERIAL_ICONS_OUTLINED] = Path.Join(AppContext.BaseDirectory, "Assets", "Fonts", "MaterialIconsOutlined-Regular.otf")
                }
            }
        };

    public required StyleSheet Default { get; init; }
}
