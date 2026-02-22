using System.Diagnostics.CodeAnalysis;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed class ColorPass(List<Entry> results) : TestPass<ColorPass, ColorPassInput, float>(results)
    {
        [AllowNull]
        public override ColorPassInput Input { get; set => field = value ?? new(); } = new();

        protected override void Execute() =>
            this.SetOutput((this.Input.Normal + this.Input.Depth + this.Input.Shadow + this.Input.Sky) * 10);
    }
}
