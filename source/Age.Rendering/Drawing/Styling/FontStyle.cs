namespace Age.Rendering.Drawing;

public record FontStyle : Trackable<FontStyle>
{
    internal override event Action? Changed;

    private TrackedValue<string> family = new("Segoe UI");
    private TrackedValue<ushort> size   = new(16);

    public string Family
    {
        get => this.family.Value;
        set => this.family.Value = value;
    }

    public ushort Size
    {
        get => this.size.Value;
        set => this.size.Value = value;
    }

    public FontStyle()
    {
        this.family.Changed += this.OnValueChanged;
        this.size.Changed   += this.OnValueChanged;
    }

    public static FontStyle Merge(FontStyle left, FontStyle right) =>
        new()
        {
            family = GetValue(left.family, right.family),
            size   = GetValue(left.size,   right.size),
        };

    private void OnValueChanged() =>
        this.Changed?.Invoke();

    public override FontStyle Merge(FontStyle other) =>
        new()
        {
            family = GetValue(this.family, other.family),
            size   = GetValue(this.size,   other.size),
        };

    public override void Update(FontStyle source)
    {
        SetValue(ref this.family, source.family);
        SetValue(ref this.size,   source.size);
    }
}
