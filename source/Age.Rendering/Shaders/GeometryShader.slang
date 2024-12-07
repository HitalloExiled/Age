struct UniformBufferObject
{
    float4x4 Model;
    float4x4 View;
    float4x4 Proj;
};

cbuffer ubo : register(b0)
{
    UniformBufferObject ubo;
}

Sampler2D sampledTexture : register(b1);

struct VertexInput
{
    float3 Position : LOCATION0;
    float3 Color    : LOCATION1;
    float2 UV       : LOCATION2;
};

struct VertexOutput
{
    float4 Position : SV_Position;
    float3 Color    : LOCATION0;
    float2 UV       : LOCATION1;
};

struct FragmentInput
{
    float3 Color : LOCATION0;
    float2 UV    : LOCATION1;
};

[shader("vertex")]
VertexOutput main(VertexInput input)
{
    var worldPosition = mul(float4(input.Position, 1.0), ubo.Model);
    var viewPosition  = mul(worldPosition, ubo.View);
    var clipPosition  = mul(viewPosition, ubo.Proj);

    VertexOutput output =
    {
        clipPosition,
        input.Color,
        input.UV
    };

    return output;
}

[shader("fragment")]
float4 main(FragmentInput input) : SV_Target
{
    return sampledTexture.Sample(input.UV);
}
