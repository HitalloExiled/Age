using System.Runtime.CompilerServices;
using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    [Fact]
    public void Execute()
    {
        var pipeline = new Graphs.RenderGraph(null!, "Rasterization");

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
        var pipeline = new Graphs.RenderGraph(null!, "Rasterization");

        var normalPass = new NormalPass([]);
        var depthPass  = new DepthPass([]);
        var shadowPass = new ShadowPass([]);

        pipeline.Connect<NormalPass, DepthPass, float>(normalPass, depthPass);
        pipeline.Connect<DepthPass, ShadowPass, float>(depthPass, shadowPass);

        Assert.Throws<RenderGraphCiclicException>(() => pipeline.Connect<ShadowPass, NormalPass, float>(shadowPass, normalPass));
    }
}
