namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private record struct Entry(string Pass, string? Input, string? Output)
    {
        public readonly override string ToString() =>
            $"{this.Pass} - in: {this.Input}, out: {this.Output}";
    }
}
