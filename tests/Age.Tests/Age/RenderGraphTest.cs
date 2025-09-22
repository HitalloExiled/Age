using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Graphs;

namespace Age.Tests.Age;

public record ColorPassInput
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

public abstract class TestPass<TThis, TOutput>(List<Entry> results) : RenderGraphNode<TOutput>
where TOutput : new()
{
    public override TOutput? Output { get; set; }

    protected override void AfterExecute() =>
        results.Add(new(typeof(TThis).Name, null, this.Output?.ToString()));
}

public abstract class TestPass<TThis, TInput, TOutput>(List<Entry> results) : RenderGraphNode<TInput, TOutput>
{
    public override TInput?  Input  { get; set; }
    public override TOutput? Output { get; set; }

    protected override void AfterExecute() =>
        results.Add(new(typeof(TThis).Name, this.Input?.ToString(), this.Output?.ToString()));
}

public sealed class GeometryPass(List<Entry> results) : TestPass<GeometryPass, float>(results)
{
    protected override void Execute(RenderContext context) =>
        this.Output = 2;
}

public sealed class EnvironmentPass(List<Entry> results) : TestPass<EnvironmentPass, float>(results)
{
    protected override void Execute(RenderContext context) =>
        this.Output = 4;
}

public sealed class NormalPass(List<Entry> results) : TestPass<NormalPass, float, float>(results)
{
    protected override void Execute(RenderContext context) =>
        this.Output = this.Input * 2;
}

public sealed class DepthPass(List<Entry> results) : TestPass<DepthPass, float, float>(results)
{
    protected override void Execute(RenderContext context) =>
        this.Output = this.Input * 3;
}

public sealed class ShadowPass(List<Entry> results) : TestPass<ShadowPass, float, float>(results)
{
    protected override void Execute(RenderContext context) =>
        this.Output = this.Input * 4;
}

public sealed class ColorPass(List<Entry> results) : TestPass<ColorPass, ColorPassInput, float>(results)
{
    [AllowNull]
    public override ColorPassInput Input { get; set => field = value ?? new(); } = new();

    protected override void Execute(RenderContext context) =>
        this.Output = (this.Input.Normal + this.Input.Depth + this.Input.Shadow + this.Input.Sky) * 10;
}

public sealed class PostFXPass(List<Entry> results) : TestPass<PostFXPass, float, float>(results)
{
    protected override void Execute(RenderContext context) =>
        this.Output = this.Input * 100;
}

public record struct Entry(string Pass, string? Input, string? Output)
{
    public readonly override string ToString() =>
        $"{this.Pass} - in: {this.Input}, out: {this.Output}";
}

public class RenderGraphTest
{
    [Fact]
    public void Execute()
    {
        var pipeline = new RenderGraphPipeline("Rasterization");

        var expected = new List<Entry>
        {
            new(nameof(GeometryPass),    null,                                      "2"),
            new(nameof(NormalPass),      "2",                                       "4"),
            new(nameof(DepthPass),       "2",                                       "6"),
            new(nameof(ShadowPass),      "2",                                       "8"),
            new(nameof(EnvironmentPass), null,                                      "4"),
            new(nameof(ColorPass),       new ColorPassInput(4, 6, 8, 4).ToString(), "220"),
            new(nameof(PostFXPass),      "220",                                     "22000"),
        };

        var actual = new List<Entry>();

        var geometryPass    = new GeometryPass(actual);
        var environmentPass = new EnvironmentPass(actual);
        var normalPass      = new NormalPass(actual);
        var depthPass       = new DepthPass(actual);
        var shadowPass      = new ShadowPass(actual);
        var colorPass       = new ColorPass(actual);
        var postFXPass      = new PostFXPass(actual);

        var geometry = pipeline.Connect(geometryPass);
        var environment = pipeline.Connect(environmentPass);

        geometry
            .Connect<NormalPass, float>(normalPass)
            .Connect<ColorPass, float>(colorPass, static (to, value) => to.Input.Normal = value);

        geometry
            .Connect<DepthPass, float>(depthPass)
            .Connect<ColorPass, float>(colorPass, static (to, value) => to.Input.Depth = value);

        geometry
            .Connect<ShadowPass, float>(shadowPass)
            .Connect<ColorPass, float>(colorPass, static (to, value) => to.Input.Shadow = value);

        environment
            .Connect(colorPass, static from => from.Output, static (to, value) => to.Input.Sky = value);

        pipeline.Connect<ColorPass, PostFXPass, float>(colorPass, postFXPass);
        pipeline.Connect<ColorPass, PostFXPass, float>(colorPass, postFXPass, static (to, value) => to.Input = value); // Emulate multiples parameters

        executeAndAssert();

        pipeline.Disconnect(depthPass);

        expected =
        [
            new(nameof(GeometryPass),    null,                                      "2"),
            new(nameof(NormalPass),      "2",                                       "4"),
            new(nameof(ShadowPass),      "2",                                       "8"),
            new(nameof(EnvironmentPass), null,                                      "4"),
            new(nameof(ColorPass),       new ColorPassInput(4, 0, 8, 4).ToString(), "160"),
            new(nameof(PostFXPass),      "160",                                     "16000"),
        ];

        executeAndAssert();

        pipeline.Disconnect(shadowPass, colorPass);

        expected =
        [
            new(nameof(GeometryPass),    null,                                      "2"),
            new(nameof(NormalPass),      "2",                                       "4"),
            new(nameof(EnvironmentPass), null,                                      "4"),
            new(nameof(ColorPass),       new ColorPassInput(4, 0, 0, 4).ToString(), "80"),
            new(nameof(PostFXPass),      "80",                                      "8000"),
            new(nameof(ShadowPass),      "2",                                       "8"),
        ];

        executeAndAssert();

        pipeline.Connect<ShadowPass, PostFXPass, float>(shadowPass, postFXPass);

        expected =
        [
            new(nameof(GeometryPass),    null,                                      "2"),
            new(nameof(NormalPass),      "2",                                       "4"),
            new(nameof(EnvironmentPass), null,                                      "4"),
            new(nameof(ColorPass),       new ColorPassInput(4, 0, 0, 4).ToString(), "80"),
            new(nameof(ShadowPass),      "2",                                       "8"),
            new(nameof(PostFXPass),      "8",                                       "800"),
        ];

        executeAndAssert();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void executeAndAssert()
        {
            actual.Clear();

            pipeline.Execute();

            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void ThrowIfCiclic()
    {
        var pipeline = new RenderGraphPipeline("Rasterization");

        var normalPass = new NormalPass([]);
        var depthPass  = new DepthPass([]);
        var shadowPass = new ShadowPass([]);

        pipeline.Connect<NormalPass, DepthPass, float>(normalPass, depthPass);
        pipeline.Connect<DepthPass, ShadowPass, float>(depthPass, shadowPass);

        Assert.Throws<RenderGraphCiclicException>(() => pipeline.Connect<ShadowPass, NormalPass, float>(shadowPass, normalPass));
    }
}
