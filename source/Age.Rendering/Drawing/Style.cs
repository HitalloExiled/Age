using Age.Numerics;
using Age.Rendering.Interfaces;

namespace Age.Rendering.Drawing;

public record Style : IMergeable<Style>
{
    private const string DEFAULT_FONT_FAMILY = "Segoe UI";
    private const ushort DEFAULT_FONT_SIZE   = 12;

    private TrackedValue<BorderStyle> border     = new(new());
    private TrackedValue<Color>       color;
    private TrackedValue<string>      fontFamily = new(DEFAULT_FONT_FAMILY);
    private TrackedValue<ushort>      fontSize   = new(DEFAULT_FONT_SIZE);
    private TrackedValue<Point<int>>  position;
    private TrackedValue<Size<uint>>  size;
    private TrackedValue<StackMode>   stack;

    internal bool HasChanges { get; set; }

    public BorderStyle Border     { get => this.border.Value;     set => this.border.Value = value; }
    public Color       Color      { get => this.color.Value;      set => this.color.Value = value; }
    public string      FontFamily { get => this.fontFamily.Value; set => this.fontFamily.Value = value; }
    public ushort      FontSize   { get => this.fontSize.Value;   set => this.fontSize.Value = value; }
    public Point<int>  Position   { get => this.position.Value;   set => this.position.Value = value; }
    public Size<uint>  Size       { get => this.size.Value;       set => this.size.Value = value; }
    public StackMode   Stack      { get => this.stack.Value;      set => this.stack.Value = value; }

    public Style()
    {
        this.border.Changed       += this.OnValueChanged;
        this.border.Value.Changed += this.OnValueChanged;
        this.color.Changed        += this.OnValueChanged;
        this.fontFamily.Changed   += this.OnValueChanged;
        this.fontSize.Changed     += this.OnValueChanged;
        this.position.Changed     += this.OnValueChanged;
        this.size.Changed         += this.OnValueChanged;
        this.stack.Changed        += this.OnValueChanged;
    }

    private void OnValueChanged() =>
        this.HasChanges = true;

    public static Style Merge(Style left, Style right) => new()
    {
        Border     = BorderStyle.Merge(left.Border, right.Border),
        color      = TrackedValue.GetValue(left.color,      right.color),
        fontFamily = TrackedValue.GetValue(left.fontFamily, right.fontFamily, DEFAULT_FONT_FAMILY),
        fontSize   = TrackedValue.GetValue(left.fontSize,   right.fontSize,   DEFAULT_FONT_SIZE),
        position   = TrackedValue.GetValue(left.position,   right.position),
        size       = TrackedValue.GetValue(left.size,       right.size),
        stack      = TrackedValue.GetValue(left.stack,      right.stack),
    };

    public static void Update(Style source, Style target)
    {
        target.Border.Update(source.Border);

        TrackedValue.SetValue(ref target.color,      source.color);
        TrackedValue.SetValue(ref target.fontFamily, source.fontFamily);
        TrackedValue.SetValue(ref target.fontSize,   source.fontSize);
        TrackedValue.SetValue(ref target.position,   source.position);
        TrackedValue.SetValue(ref target.size,       source.size);
        TrackedValue.SetValue(ref target.stack,      source.stack);
    }

    public void Update(Style source) =>
        Update(source, this);

    public Style Merge(Style other) =>
        Merge(other, this);
}
