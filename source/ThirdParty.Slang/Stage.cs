namespace ThirdParty.Slang;

public enum Stage : uint
{
    None,
    Vertex,
    Hull,
    Domain,
    Geometry,
    Fragment,
    Compute,
    RayGeneration,
    Intersection,
    AnyHit,
    ClosestHit,
    Miss,
    Callable,
    Mesh,
    Amplification,

    // alias:
    Pixel = Fragment,
};
