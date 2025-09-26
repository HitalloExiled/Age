using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class ShadowPass(List<Entry> results) : TestPass<ShadowPass, float, float>(results)
    {
        protected override void Execute(RenderContext context) =>
            this.Output = this.Input * 4;
    }
}
