using Age.Rendering.Enums;

namespace Age.Rendering;

public abstract record Uniform
{
    public abstract UniformType Type { get; }

    public required uint Binding { get; init; }

    public bool Bounded { get; set; }
}
