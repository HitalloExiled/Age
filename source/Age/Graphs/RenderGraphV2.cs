namespace Age.Graphs;

public static class RenderGraphV2
{
    private static readonly List<RenderGraphPipeline> pipelines = [];
    public static IReadOnlyList<RenderGraphPipeline> Pipelines => pipelines;

    public static void AddPipeline(RenderGraphPipeline pipeline) =>
        pipelines.Add(pipeline);
}
