namespace Age.Storage;

public static class IconExtensions
{
    extension(Icon)
    {
        public static Icon Parse(scoped ReadOnlySpan<char> name) =>
            name switch
            {
                var value when value.Equals(nameof(Icon.Block),                StringComparison.OrdinalIgnoreCase) => Icon.Block,
                var value when value.Equals(nameof(Icon.Cancel),               StringComparison.OrdinalIgnoreCase) => Icon.Cancel,
                var value when value.Equals(nameof(Icon.CheckBoxOutlineBlank), StringComparison.OrdinalIgnoreCase) => Icon.CheckBoxOutlineBlank,
                var value when value.Equals(nameof(Icon.CheckBox),             StringComparison.OrdinalIgnoreCase) => Icon.CheckBox,
                var value when value.Equals(nameof(Icon.CheckCircle),          StringComparison.OrdinalIgnoreCase) => Icon.CheckCircle,
                var value when value.Equals(nameof(Icon.Help),                 StringComparison.OrdinalIgnoreCase) => Icon.Help,
                var value when value.Equals(nameof(Icon.Info),                 StringComparison.OrdinalIgnoreCase) => Icon.Info,
                var value when value.Equals(nameof(Icon.RadioButtonChecked),   StringComparison.OrdinalIgnoreCase) => Icon.RadioButtonChecked,
                var value when value.Equals(nameof(Icon.RadioButtonUnchecked), StringComparison.OrdinalIgnoreCase) => Icon.RadioButtonUnchecked,
                var value when value.Equals(nameof(Icon.Refresh),              StringComparison.OrdinalIgnoreCase) => Icon.Refresh,
                var value when value.Equals(nameof(Icon.Save),                 StringComparison.OrdinalIgnoreCase) => Icon.Save,
                _ => throw new NotSupportedException(),
            };
    }

    extension (Icon value)
    {
        public string GetName() =>
            value switch
            {
                Icon.Block                => nameof(Icon.Block),
                Icon.Cancel               => nameof(Icon.Cancel),
                Icon.CheckBoxOutlineBlank => nameof(Icon.CheckBoxOutlineBlank),
                Icon.CheckBox             => nameof(Icon.CheckBox),
                Icon.CheckCircle          => nameof(Icon.CheckCircle),
                Icon.Help                 => nameof(Icon.Help),
                Icon.Info                 => nameof(Icon.Info),
                Icon.RadioButtonChecked   => nameof(Icon.RadioButtonChecked),
                Icon.RadioButtonUnchecked => nameof(Icon.RadioButtonUnchecked),
                Icon.Refresh              => nameof(Icon.Refresh),
                Icon.Save                 => nameof(Icon.Save),
                _ => throw new NotSupportedException(),
            };
    }
}
