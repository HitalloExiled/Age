namespace Age.Commands;

[Flags]
public enum PipelineVariant : uint
{
    None      = 0,
    Color     = 1 << 0,
    Index     = 1 << 1,
    Stencil   = 1 << 2,
    Wireframe = 1 << 3,
}
