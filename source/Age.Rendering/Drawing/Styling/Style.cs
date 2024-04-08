using Age.Numerics;

namespace Age.Rendering.Drawing;

public record Style : Trackable<Style>
{
    internal override event Action? Changed;

    private readonly BorderStyle border = new();
    private readonly FontStyle   font   = new();

    private TrackedValue<Color>      color;
    private TrackedValue<Point<int>> position;
    private TrackedValue<Size<uint>> size;
    private TrackedValue<StackMode>  stack = new(StackMode.Horizontal);

    public BorderStyle Border
    {
        get => this.border;
        set => this.border.Update(value);
    }

    public Color Color
    {
        get => GetValue(this.color, this.Parent?.color).Value;
        set => this.color.Value = value;
    }

    public FontStyle Font
    {
        get => this.font;
        set => this.font.Update(value);
    }

    public Point<int> Position
    {
        get => GetValue(this.position, this.Parent?.position).Value;
        set => this.position.Value = value;
    }

    public Size<uint> Size
    {
        get => GetValue(this.size, this.Parent?.size).Value;
        set => this.size.Value = value;
    }

    public StackMode Stack
    {
        get => GetValue(this.stack, this.Parent?.stack).Value;
        set => this.stack.Value = value;
    }

    private Style? parent;

    internal override Style? Parent
    {
        get => this.parent;
        set
        {
            this.parent        = value;
            this.Border.Parent = value?.Border;
            this.Font.Parent   = value?.Font;
        }
    }

    public Style() =>
        this.ListenEvents();

    public Style(Style source) : base(source)
    {
        this.border = new();
        this.font   = new();

        this.border.Update(source.border);
        this.font.Update(source.font);

        this.color.Value    = source.color.Value;
        this.position.Value = source.position.Value;
        this.size.Value     = source.size.Value;
        this.stack.Value    = source.stack.Value;

        this.Parent = source.Parent;

        this.ListenEvents();
    }

    private void ListenEvents()
    {
        this.border.Changed   += this.OnValueChanged;
        this.color.Changed    += this.OnValueChanged;
        this.font.Changed     += this.OnValueChanged;
        this.position.Changed += this.OnValueChanged;
        this.size.Changed     += this.OnValueChanged;
        this.stack.Changed    += this.OnValueChanged;
    }

    private void OnValueChanged() =>
        this.Changed?.Invoke();

    public override void Update(Style source)
    {
        SetValue(ref this.color,    source.color);
        SetValue(ref this.position, source.position);
        SetValue(ref this.size,     source.size);
        SetValue(ref this.stack,    source.stack);

        this.Border.Update(source.Border);
        this.Font.Update(source.Font);
    }

    public override Style Merge(Style other) =>
        new()
        {
            color    = GetValue(this.color,      other.color),
            position = GetValue(this.position,   other.position),
            size     = GetValue(this.size,       other.size),
            stack    = GetValue(this.stack,      other.stack),
            Border   = this.Border.Merge(other.Border),
            Font     = this.Font.Merge(other.Font),
        };
}
