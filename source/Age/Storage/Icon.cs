using Age.Core.Collections;

namespace Age.Storage;

[KeyedListKey]
public enum Icon
{
    None = 0,
    Block                = 1 << 0,
    Cancel               = 1 << 1,
    CheckBoxOutlineBlank = 1 << 2,
    CheckBox             = 1 << 3,
    CheckCircle          = 1 << 4,
    Help                 = 1 << 5,
    Info                 = 1 << 6,
    RadioButtonChecked   = 1 << 7,
    RadioButtonUnchecked = 1 << 8,
    Refresh              = 1 << 9,
    Save                 = 1 << 10,
}
