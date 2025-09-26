using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class EnvironmentPass(List<Entry> results) : TestPass<EnvironmentPass, float>(results)
    {
        protected override void Execute(RenderContext context) =>
            this.Output = 4;
    }
}
