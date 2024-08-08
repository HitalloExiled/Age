namespace Age.Styling;

public readonly struct SizeUnit(float value, UnitType type)
{
    public readonly UnitType Type  = type;
    public readonly float    Value = value;

    public static SizeUnit Pc(float value) => new(value, UnitType.Pc);
    public static SizeUnit Px(float value) => new(value, UnitType.Px);

    public static implicit operator SizeUnit(float value) => new(value, UnitType.Px);
    public static implicit operator float(SizeUnit size)  => size.Value;
}
