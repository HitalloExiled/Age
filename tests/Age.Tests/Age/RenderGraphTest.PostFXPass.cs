using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class PostFXPass(List<Entry> results) : TestPass<PostFXPass, float, float>(results)
    {
        protected override void Execute(RenderContext context) =>
            this.SetOutput(this.Input * 100);
    }
}
