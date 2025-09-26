using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class NormalPass(List<Entry> results) : TestPass<NormalPass, float, float>(results)
    {
        protected override void Execute(RenderContext context) =>
            this.SetOutput(this.Input * 2);
    }
}
