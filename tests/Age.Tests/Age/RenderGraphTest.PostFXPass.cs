using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class PostFXPass(List<Entry> results) : TestPass<PostFXPass, float, float>(results)
    {
        protected override void Execute(RenderContext context) =>
            this.Output = this.Input * 100;
    }
}
