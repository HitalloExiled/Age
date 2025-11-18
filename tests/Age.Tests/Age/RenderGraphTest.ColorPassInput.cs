namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private sealed record ColorPassInput
    {
        public float Depth  { get; set; }
        public float Normal { get; set; }
        public float Shadow { get; set; }
        public float Sky    { get; set; }

        public ColorPassInput() { }

        public ColorPassInput(float normal, float depth, float shadow, float sky)
        {
            this.Normal = normal;
            this.Depth  = depth;
            this.Shadow = shadow;
            this.Sky    = sky;
        }

        public override string ToString() =>
            $"{{ Normal: {this.Normal}, Depth: {this.Depth}, Shadow: {this.Shadow}, Sky: {this.Sky} }}";
    }
}
