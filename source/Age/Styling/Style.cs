using System.Runtime.CompilerServices;
using System.Text;
using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Platforms.Display;

namespace Age.Styling;

public record Style
{
    internal event Action<StyleProperty>? Changed;

    private readonly Dictionary<StyleProperty, int>               values  = [];
    private readonly Dictionary<StyleProperty, NativeArray<byte>> structs = [];

    private static readonly NativeArrayPool<byte> arrayPool = new();

    public AlignmentKind? Alignment
    {
        get => this.GetEnum<AlignmentKind>(StyleProperty.Alignment);
        set => this.SetEnum(StyleProperty.Alignment, value);
    }

    public Color? BackgroundColor
    {
        get => this.GetStruct<Color>(StyleProperty.BackgroundColor);
        set => this.SetStruct(StyleProperty.BackgroundColor, ref value);
    }

    public Unit? Baseline
    {
        get => this.GetStruct<Unit>(StyleProperty.Baseline);
        set => this.SetStruct(StyleProperty.Baseline, ref value);
    }

    public Border? Border
    {
        get => this.GetStruct<Border>(StyleProperty.Border);
        set => this.SetStruct(StyleProperty.Border, ref value);
    }

    public BoxSizing? BoxSizing
    {
        get => this.GetEnum<BoxSizing>(StyleProperty.BoxSizing);
        set => this.SetEnum(StyleProperty.BoxSizing, value);
    }

    public Color? Color
    {
        get => this.GetStruct<Color>(StyleProperty.Color);
        set => this.SetStruct(StyleProperty.Color, ref value);
    }

    public ContentJustificationKind? ContentJustification
    {
        get => this.GetEnum<ContentJustificationKind>(StyleProperty.ContentJustification);
        set => this.SetEnum(StyleProperty.ContentJustification, value);
    }

    public CursorKind? Cursor
    {
        get => this.GetEnum<CursorKind>(StyleProperty.Cursor);
        set => this.SetEnum(StyleProperty.Cursor, value);
    }

    public string? FontFamily
    {
        get => this.GetString(StyleProperty.FontFamily);
        set => this.SetString(StyleProperty.FontFamily, value);
    }

    public uint? FontSize
    {
        get => (uint?)this.GetValue(StyleProperty.FontSize);
        set => this.SetValue(StyleProperty.FontSize, (int?)value);
    }

    public FontWeight? FontWeight
    {
        get => (FontWeight?)this.GetValue(StyleProperty.FontWeight);
        set => this.SetValue(StyleProperty.FontWeight, (int?)value);
    }

    public bool? Hidden
    {
        get => this.GetBool(StyleProperty.FontWeight);
        set => this.SetBool(StyleProperty.FontWeight, value);
    }

    public ItemsAlignmentKind? ItemsAlignment
    {
        get => this.GetEnum<ItemsAlignmentKind>(StyleProperty.ItemsAlignment);
        set => this.SetEnum(StyleProperty.ItemsAlignment, value);
    }

    public StyleRectEdges? Margin
    {
        get => this.GetStruct<StyleRectEdges>(StyleProperty.Margin);
        set => this.SetStruct(StyleProperty.Margin, ref value);
    }

    public SizeUnit? MaxSize
    {
        get => this.GetStruct<SizeUnit>(StyleProperty.MaxSize);
        set => this.SetStruct(StyleProperty.MaxSize, ref value);
    }

    public SizeUnit? MinSize
    {
        get => this.GetStruct<SizeUnit>(StyleProperty.MinSize);
        set => this.SetStruct(StyleProperty.MinSize, ref value);
    }

    public OverflowKind? Overflow
    {
        get => this.GetEnum<OverflowKind>(StyleProperty.Overflow);
        set => this.SetEnum(StyleProperty.Overflow, value);
    }

    public StyleRectEdges? Padding
    {
        get => this.GetStruct<StyleRectEdges>(StyleProperty.Padding);
        set => this.SetStruct(StyleProperty.Padding, ref value);
    }

    public PositionKind? Positioning
    {
        get => this.GetEnum<PositionKind>(StyleProperty.Positioning);
        set => this.SetEnum(StyleProperty.Positioning, value);
    }

    public SizeUnit? Size
    {
        get => this.GetStruct<SizeUnit>(StyleProperty.Size);
        set => this.SetStruct(StyleProperty.Size, ref value);
    }

    public StackKind? Stack
    {
        get => this.GetEnum<StackKind>(StyleProperty.Stack);
        set => this.SetEnum(StyleProperty.Stack, value);
    }

    public TextAlignmentKind? TextAlignment
    {
        get => this.GetEnum<TextAlignmentKind>(StyleProperty.TextAlignment);
        set => this.SetEnum(StyleProperty.TextAlignment, value);
    }

    public bool? TextSelection
    {
        get => this.GetBool(StyleProperty.TextSelection);
        set => this.SetBool(StyleProperty.TextSelection, value);
    }

    public Transform2D? Transform
    {
        get => this.GetStruct<Transform2D>(StyleProperty.Transform);
        set => this.SetStruct(StyleProperty.Transform, ref value);
    }

    public Style(Style style)
    {
        this.values  = new(style.values);
        this.structs = [];

        foreach (var (key, value) in style.structs)
        {
            this.structs[key] = arrayPool.GetCopy(value);
        }
    }

    public static StyleProperty Diff(Style left, Style right)
    {
        StyleProperty changes = default;

        foreach (var (key, value) in right.values)
        {
            if (!left.values.TryGetValue(key, out var entry) || entry != value)
            {
                changes |= key;
            }
        }

        foreach (var (key, value) in right.structs)
        {
            if (!left.structs.TryGetValue(key, out var entry) || !entry.AsSpan().SequenceEqual(value))
            {
                changes |= key;
            }
        }

        foreach (var (key, value) in left.values)
        {
            if (changes.HasFlags(key))
            {
                continue;
            }

            if (!right.values.ContainsKey(key))
            {
                changes |= key;
            }
        }

        foreach (var (key, value) in left.structs)
        {
            if (changes.HasFlags(key))
            {
                continue;
            }

            if (!right.structs.ContainsKey(key))
            {
                changes |= key;
            }
        }

        return changes;
    }

    public static Style Merge(Style left, Style right)
    {
        var style = new Style(left);

        foreach (var (key, value) in right.values)
        {
            style.values.TryAdd(key, value);
        }

        foreach (var (key, value) in right.structs)
        {
            ref var entry = ref style.structs.GetValueRefOrAddDefault(key, out var exists);

            if (!exists)
            {
                entry = arrayPool.GetCopy(value);
            }
        }

        return style;
    }

    private static void CopyString(StyleProperty property, Style source, Style target)
    {
        if (source.structs.TryGetValue(property, out var sourceString))
        {
            ref var targetString = ref target.structs.GetValueRefOrAddDefault(property, out var exists);

            if (!exists)
            {
                targetString = arrayPool.GetCopy(sourceString);
            }
            else
            {
                if (targetString!.Length != sourceString.Length)
                {
                    targetString.Resize(sourceString.Length);
                }

                sourceString.CopyTo(targetString!);
            }
        }
        else
        {
            target.structs.Remove(property);
        }
    }

    private static void CopyStruct(StyleProperty property, Style source, Style target)
    {
        if (source.structs.TryGetValue(property, out var sourceSruct))
        {
            ref var targetStruct = ref target.structs.GetValueRefOrAddDefault(property, out var exists);

            if (!exists)
            {
                targetStruct = arrayPool.GetCopy(sourceSruct);
            }
            else
            {
                sourceSruct.CopyTo(targetStruct!);
            }
        }
        else
        {
            target.structs.Remove(property);
        }
    }

    private static void CopyValue(StyleProperty property, Style source, Style target)
    {
        if (source.values.TryGetValue(property, out var smallEntry))
        {
            target.values[property] = smallEntry;
        }
        else
        {
            target.values.Remove(property);
        }
    }

    private bool? GetBool(StyleProperty property) =>
        this.GetValue(property) is int value ? value == 1 : null;

    private T? GetEnum<T>(StyleProperty property) where T : struct, Enum =>
        this.values.TryGetValue(property, out var value) ? Unsafe.As<int, T>(ref value) : null;

    private string? GetString(StyleProperty property) =>
        this.structs.TryGetValue(property, out var buffer) ? new(buffer.AsSpan().Cast<byte, char>()) : null;

    private T? GetStruct<T>(StyleProperty property) where T : unmanaged =>
        this.structs.TryGetValue(property, out var buffer) ? buffer.AsSpan().AsStructRef<T>() : null;

    private int? GetValue(StyleProperty property) =>
        this.values.TryGetValue(property, out var value) ? value : null;

    private void SetBool(StyleProperty property, bool? value) =>
        this.SetValue(property, value.HasValue ? value.Value ? 1 : 0 : null);

    private unsafe void SetEnum<T>(StyleProperty property, T? value) where T : unmanaged, Enum
    {
        if (sizeof(T) != sizeof(int))
        {
            throw new InvalidOperationException("The provided enum type must have an underlying type of int.");
        }

        this.SetValue(property, Unsafe.As<T?, int?>(ref value));
    }

    private unsafe void SetStruct<T>(StyleProperty property, ref T? value) where T : unmanaged, IEquatable<T>
    {
        if (!value.HasValue)
        {
            if (this.structs.Remove(property, out var entry))
            {
                arrayPool.Return(entry);
            }
        }
        else
        {
            ref var buffer = ref this.structs.GetValueRefOrAddDefault(property, out var exists);

            if (!exists)
            {
                buffer = arrayPool.Get(sizeof(T));
            }

            ref var left = ref buffer!.AsSpan().AsStructRef<T>();
            ref readonly var right = ref Nullable.GetValueRefOrDefaultRef(ref value);

            if (!exists || !left.Equals(right))
            {
                left = right;

                this.Changed?.Invoke(property);
            }
        }
    }

    private unsafe void SetString(StyleProperty property, string? value)
    {
        if (value == null)
        {
            if (this.structs.Remove(property, out var entry))
            {
                arrayPool.Return(entry);
            }
        }
        else
        {
            ref var entry = ref this.structs.GetValueRefOrAddDefault(property, out var exists);

            var buffer = value.AsSpan().Cast<char, byte>();

            if (!exists)
            {
                entry = arrayPool.Get(buffer.Length);
            }
            else if (entry!.Length != buffer.Length)
            {
                entry.Resize(buffer.Length);
            }

            if (!exists || !buffer.SequenceEqual(entry!))
            {
                buffer.CopyTo(entry!);

                this.Changed?.Invoke(property);
            }
        }
    }

    private unsafe void SetValue(StyleProperty property, int? value)
    {
        if (!value.HasValue)
        {
            this.values.Remove(property);
        }
        else
        {
            ref var entry = ref this.values.GetValueRefOrAddDefault(property, out var exists);

            if (!exists || entry != value)
            {
                entry = value.Value;
                this.Changed?.Invoke(property);
            }
        }
    }

    internal void Clear()
    {
        foreach (var entry in this.structs.Values)
        {
            arrayPool.Return(entry);
        }

        this.values.Clear();
        this.structs.Clear();
    }

    internal void Copy(Style source)
    {
        this.Clear();
        this.Merge(source);
    }

    internal void Copy(Style source, StyleProperty property)
    {
        switch (property)
        {
            case StyleProperty.FontFamily:
                CopyString(property, source, this);
                break;

            case StyleProperty.BackgroundColor:
            case StyleProperty.Border:
            case StyleProperty.Color:
            case StyleProperty.Margin:
            case StyleProperty.MaxSize:
            case StyleProperty.MinSize:
            case StyleProperty.Padding:
            case StyleProperty.Size:
            case StyleProperty.Transform:
                CopyStruct(property, source, this);
                break;

            default:
                CopyValue(property, source, this);
                break;
        }
    }

    internal void Merge(Style source)
    {
        foreach (var (key, value) in source.values)
        {
            this.values[key] = value;
        }

        foreach (var (key, value) in source.structs)
        {
            this.structs[key] = arrayPool.GetCopy(value);
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        void appendProperty<T>(string name, in T? value)
        {
            if (value != null)
            {
                builder.Append($"{name}: {value}");
                builder.Append("; ");
            }
        }

        appendProperty(nameof(StyleProperty.Alignment),            this.Alignment);
        appendProperty(nameof(StyleProperty.BackgroundColor),      this.BackgroundColor);
        appendProperty(nameof(StyleProperty.Baseline),             this.Baseline);
        appendProperty(nameof(StyleProperty.Border),               this.Border);
        appendProperty(nameof(StyleProperty.BoxSizing),            this.BoxSizing);
        appendProperty(nameof(StyleProperty.Color),                this.Color);
        appendProperty(nameof(StyleProperty.ContentJustification), this.ContentJustification);
        appendProperty(nameof(StyleProperty.Cursor),               this.Cursor);
        appendProperty(nameof(StyleProperty.FontFamily),           this.FontFamily);
        appendProperty(nameof(StyleProperty.FontSize),             this.FontSize);
        appendProperty(nameof(StyleProperty.FontWeight),           this.FontWeight);
        appendProperty(nameof(StyleProperty.Hidden),               this.Hidden);
        appendProperty(nameof(StyleProperty.ItemsAlignment),       this.ItemsAlignment);
        appendProperty(nameof(StyleProperty.Margin),               this.Margin);
        appendProperty(nameof(StyleProperty.MaxSize),              this.MaxSize);
        appendProperty(nameof(StyleProperty.MinSize),              this.MinSize);
        appendProperty(nameof(StyleProperty.Overflow),             this.Overflow);
        appendProperty(nameof(StyleProperty.Padding),              this.Padding);
        appendProperty(nameof(StyleProperty.Positioning),          this.Positioning);
        appendProperty(nameof(StyleProperty.Size),                 this.Size);
        appendProperty(nameof(StyleProperty.Stack),                this.Stack);
        appendProperty(nameof(StyleProperty.TextAlignment),        this.TextAlignment);
        appendProperty(nameof(StyleProperty.TextSelection),        this.TextSelection);
        appendProperty(nameof(StyleProperty.Transform),            this.Transform);

        if (builder.Length > 0)
        {
            builder.Remove(builder.Length - 2, 2);
        }

        return builder.ToString();
    }
}
