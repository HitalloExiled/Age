import "Modules/Numerics";

import "CanvasShader.Border";
import "CanvasShader.Flags";

public struct PushConstant
{
    // [16-bytes boundary]
    public float4 color;

    // [8-bytes boundary]
    public float2    viewport;
    public float2    size;
    public Transform transform;
    public float2    uv[4];
    public Border    border;

    // [4-bytes boundary]
    public Flags flags;
}
