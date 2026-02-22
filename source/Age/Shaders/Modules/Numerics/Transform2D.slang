implementing Numerics;

public struct Transform2D
{
    public float2x2 rotation;
    public float2   position;

    public float3x3 toFloat3x3()
    {
        return float3x3(
            float3(this.rotation[0], 0),
            float3(this.rotation[1], 0),
            float3(this.position, 1)
        );
    }
}
