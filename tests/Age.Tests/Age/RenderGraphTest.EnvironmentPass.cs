namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class EnvironmentPass(List<Entry> results) : TestPass<EnvironmentPass, float>(results)
    {
        protected override void Execute() =>
            this.SetOutput(4);
    }
}
