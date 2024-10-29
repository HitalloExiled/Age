namespace Age.Commands;

public enum PipelineVariant : uint
{
    Color     = 1 << 0,
    Index     = 1 << 1,
    Stencil   = 1 << 2,
    Wireframe = 1 << 3,
}
