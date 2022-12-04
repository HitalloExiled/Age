using Age.Core.Math;
using Age.Drivers.GLES3;
using Age.Servers.Rendering.Storage;

using RealT = System.Single;
using RGS   = Age.Servers.RenderingServerGlobals;
using RS    = Age.Servers.RenderingServer;

namespace Age.Servers.Rendering;

#pragma warning disable CS0649,IDE0044,IDE0052,IDE0059,IDE0060 // TODO - Remove

internal record Singleton
{

}

internal enum CameraType
{
    PERSPECTIVE,
    ORTHOGONAL,
    FRUSTUM
};

internal record struct Camera
{
    public Guid               Attributes    { get; set; }
    public Guid               Environment   { get; set; }
    public RealT              Fov           { get; set; }
    public Vector2<RealT>     Offset        { get; set; }
    public RealT              Size          { get; set; }
    public Transform3D<RealT> Transform     { get; set; }
    public CameraType         Type          { get; set; }
    public bool               VAspect       { get; set; }
    public uint               VisibleLayers { get; set; }
    public RealT              ZFar          { get; set; }
    public RealT              ZNear         { get; set; }
}

internal record Scenario
{
    public enum IndexerType
    {
        INDEXER_GEOMETRY, //for geometry
        INDEXER_VOLUMES, //for everything else
        INDEXER_MAX
    };

    public DynamicBVH[] Indexers = new DynamicBVH[(int)IndexerType.INDEXER_MAX];

    public Guid Environment         { get; set; }
    public Guid FallbackEnvironment { get; set; }
}

internal class RendererSceneCull : RasterizerSceneGLES3 // TODO - Implements RenderingMethod abstractions
{
    private RendererSceneRender? sceneRender;
    private uint geometryInstancePairMask;
    public RendererSceneRender SceneRender
    {
        get
        {
            if (this.sceneRender == null)
            {
                throw new Exception();
            }

            return this.sceneRender;
        }
        set
        {
            this.geometryInstancePairMask = value.GeometryInstanceGetPairMask();
            this.sceneRender              = value;
        }
    }

    private const int TAA_JITTER_COUNT = 16;
    private readonly Dictionary<Guid, Camera>   cameras            = new();
    private readonly Dictionary<Guid, Singleton> instances          = new();
    private readonly RendererEnvironmentStorage environmentStorage = new();
    private int                                 indexerUpdateIterations;
    private int                                 renderPass;
    private readonly Queue<Singleton>            instanceUpdateList = new();
    private readonly Dictionary<Guid, Scenario> scenarios          = new();
    private readonly List<Vector2<RealT>>       taaJitterArray     = new();

    private Guid RenderGetEnvironment(Guid cameraId, Guid scenarioId)
    {
        if (this.cameras.TryGetValue(cameraId, out var camera) && this.SceneRender.IsEnvironment(camera.Environment))
        {
            return camera.Environment;
        }

        if (!this.scenarios.TryGetValue(scenarioId, out var scenario))
        {
            return default;
        }

        if (this.SceneRender.IsEnvironment(scenario.Environment))
        {
            return scenario.Environment;
        }

        if (this.SceneRender.IsEnvironment(scenario.FallbackEnvironment))
        {
            return scenario.FallbackEnvironment;
        }

        return default;
    }

    private void RenderScene(CameraData cameraData, RenderSceneBuffers renderBuffers, Guid environmentId, Guid attributes, uint visibleLayers, Guid scenarioId, Guid viewportId, Guid shadowAtlas, Guid reflectionProbeId, int reflectionProbePass, float screenMeshLodThreshold, bool usingShadows, RenderInfo renderInfo)
    {
        var renderReflectionProbe = this.instances[reflectionProbeId];
        var scenario              = this.scenarios[scenarioId];

        this.renderPass++;

        this.SceneRender.SetScenePass(this.renderPass);
    }

    public static RendererSceneCull Singleton { get; } = new();

    public bool IsCamera(Guid guid) => this.cameras.ContainsKey(guid);

    public RS.EnvironmentBG EnvironmentGetBackground(object environment) => throw new NotImplementedException();

    public int EnvironmentGetCanvasMaxLayer(object environment) => throw new NotImplementedException();

    public bool IsEnvironment(Guid environment) =>
        this.environmentStorage.IsEnvironment(environment);

    public bool IsScenario(Guid scenario) => this.scenarios.ContainsKey(scenario);

    public Guid ScenarioGetEnvironment(Guid scenarioId)
    {
        var scenario = this.scenarios[scenarioId];

        return scenario.Environment;
    }

    public void RenderCamera(
        RenderSceneBuffers renderBuffers,
        Guid               cameraId,
        Guid               scenarioId,
        Guid               viewportId,
        Vector2<RealT>     viewPortSize,
        bool               useTaa,
        RealT              screenMeshLodThreshold,
        Guid               shadowAtlas,
        object?            xrInterface, // TODO - XRInterface xrInterface,
        RenderInfo         renderInfo
    )
    {
        var camera = this.cameras[cameraId];

        var jitter = useTaa
            ? this.taaJitterArray[RGS.Rasterizer.FrameNumber % TAA_JITTER_COUNT] / viewPortSize
            : new Vector2<RealT>();

        var cameraData = new CameraData();

        if (xrInterface == null)
        {
            var transform    = camera.Transform;
            var projection   = new Projection<RealT>();
            var vaspect      = camera.VAspect;
            var isOrthogonal = false;
            var aspect       = viewPortSize.X / viewPortSize.Y;

            switch (camera.Type)
            {
                case CameraType.ORTHOGONAL:
                    projection.SetOrthogonal(
                        camera.Size,
                        aspect,
                        camera.ZNear,
                        camera.ZFar,
                        camera.VAspect
                    );
                    isOrthogonal = true;
                    break;
                case CameraType.PERSPECTIVE:
                    projection.SetPerspective(
						camera.Fov,
						aspect,
						camera.ZNear,
						camera.ZFar,
						camera.VAspect
                    );
                    break;
                case CameraType.FRUSTUM:
                    projection.SetFrustum(
						camera.Size,
						aspect,
						camera.Offset,
						camera.ZNear,
						camera.ZFar,
						camera.VAspect
                    );
                    break;
            }

            cameraData.SetCamera(transform, projection, isOrthogonal, vaspect, jitter, camera.VisibleLayers);
        }
        else
        {
            // F:\Projects\godot\servers\rendering\renderer_scene_cull.cpp[2532:2557]
        }

        var environmentId = this.RenderGetEnvironment(cameraId, scenarioId);

        // F:\Projects\godot\servers\rendering\renderer_scene_cull.cpp[2562]

        RendererSceneOcclusionCull.Singleton.BufferUpdate(viewportId, cameraData.MainTransform, cameraData.MainProjection, cameraData.IsOrthogonal);

        this.RenderScene(
            cameraData,
            renderBuffers,
            environmentId,
            camera.Attributes,
            camera.VisibleLayers,
            scenarioId,
            viewportId,
            shadowAtlas,
            default,
            -1,
            screenMeshLodThreshold,
            true,
            renderInfo
        );
    }

    public void Update()
    {
        // TODO - servers\rendering\renderer_scene_cull.cpp[3963:3965]
        foreach (var scenario in this.scenarios)
        {
            scenario.Value.Indexers[(int)Scenario.IndexerType.INDEXER_GEOMETRY].OptimizeIncremental(this.indexerUpdateIterations);
            scenario.Value.Indexers[(int)Scenario.IndexerType.INDEXER_VOLUMES].OptimizeIncremental(this.indexerUpdateIterations);
        }

        this.SceneRender.Update();
        this.UpdateDirtyInstances();
        // TODO - servers\rendering\renderer_scene_cull.cpp[3971]
    }

    private void UpdateDirtyInstance(Singleton item) => // TODO - servers\rendering\renderer_scene_cull.cpp[3716]
        throw new NotImplementedException();

    private void UpdateDirtyInstances()
    {
        RGS.Utilities.UpdateDirtyResources();

        foreach (var item in this.instanceUpdateList)
        {
            this.UpdateDirtyInstance(item);
        }
    }
}
