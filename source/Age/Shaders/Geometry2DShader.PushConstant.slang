import Modules.Numerics;

import "./Geometry2DShader.Border";
import "./Geometry2DShader.Flags";

public struct PushConstant
{
    // [16-bytes boundary]
    public float4 color;

    // [8-bytes boundary]
    public uint2       viewport;
    public uint2       size;
    public Transform2D transform;
    public float2      uv[4];
    public Border      border;

    // [4-bytes boundary]
    public Flags flags;
}
