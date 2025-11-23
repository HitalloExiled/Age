namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class DepthPass(List<Entry> results) : TestPass<DepthPass, float, float>(results)
    {
        protected override void Execute() =>
            this.SetOutput(this.Input * 3);
    }
}
