using Age.Numerics;

namespace Age.Storage;

internal partial class IconStorage
{
    public record Icon
    {
        public required IconName    Name     { get; init; }
        public required Point<uint> Position { get; init; }
        public required Size<uint>  Size     { get; init; }

        public static string GetAssetName(IconName iconName) => iconName switch
            {
                IconName.Block                => "block",
                IconName.Cancel               => "cancel",
                IconName.CheckBoxOutlineBlank => "check_box_outline_blank",
                IconName.CheckBox             => "check_box",
                IconName.CheckCircle          => "check_circle",
                IconName.Help                 => "help",
                IconName.Info                 => "info",
                IconName.RadioButtonChecked   => "radio_button_checked",
                IconName.RadioButtonUnchecked => "radio_button_unchecked",
                IconName.Refresh              => "refresh",
                IconName.Save                 => "save",
                _ => throw new InvalidOperationException(),
            };
    }
}
