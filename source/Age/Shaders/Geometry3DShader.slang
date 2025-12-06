struct UniformBufferObject
{
    float4x4 model;
    float4x4 view;
    float4x4 proj;
};

[vk::binding(0)]
cbuffer ubo
{
    UniformBufferObject ubo;
}

[vk::binding(1)]
Sampler2D sampledTexture;

struct VertexInput
{
    float3 position : LOCATION0;
    float3 color    : LOCATION1;
    float2 uv       : LOCATION2;
};

struct VertexOutput
{
    float4 position : SV_Position;
    float3 color    : LOCATION0;
    float2 uv       : LOCATION1;
};

struct FragmentInput
{
    float3 color : LOCATION0;
    float2 uv    : LOCATION1;
};

[shader("vertex")]
VertexOutput main(VertexInput input)
{
    var worldPosition = mul(float4(input.position, 1.0), ubo.model);
    var viewPosition  = mul(worldPosition, ubo.view);
    var clipPosition  = mul(viewPosition, ubo.proj);

    return
    {
        clipPosition,
        input.color,
        input.uv
    };
}

[shader("fragment")]
float4 main(FragmentInput input) : SV_Target
{
    return sampledTexture.Sample(input.uv);
}
