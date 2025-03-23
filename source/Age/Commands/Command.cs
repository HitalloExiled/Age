using Age.Elements;

namespace Age.Commands;

public abstract record Command
{
    public ulong           ObjectId        { get; set; }
    public PipelineVariant PipelineVariant { get; set; }
    internal StencilLayer? StencilLayer    { get; set; }
}
