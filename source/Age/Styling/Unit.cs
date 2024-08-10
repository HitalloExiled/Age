namespace Age.Styling;

public record struct Unit(float Value, UnitType Type)
{
    public UnitType Type  = Type;
    public float    Value = Value;

    public static Unit Pixel(float value)      => new(value, UnitType.Pixel);
    public static Unit Percentage(float value) => new(value / 100, UnitType.Percentage);

    public override readonly string ToString() =>
        this.Type == UnitType.Pixel
            ? $"{this.Value}px"
            : $"{this.Value * 100}%";
};
