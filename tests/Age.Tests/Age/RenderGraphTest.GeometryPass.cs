using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class GeometryPass(List<Entry> results) : TestPass<GeometryPass, float>(results)
    {
        protected override void Execute(RenderContext context) =>
            this.Output = 2;
    }
}
