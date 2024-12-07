struct UniformBufferObject
{
    float4x4 model;
    float4x4 view;
    float4x4 proj;
};

cbuffer ubo : register(b0)
{
    UniformBufferObject ubo;
}

// ParameterBlock<UniformBufferObject> ubo;

// Texture2D    texSampler      : register(t1);
// SamplerState texSamplerState : register(s1);
Sampler2D texSampler : register(t1) : register(s1);

struct VertexInput
{
    float3 Position : POSITION;
    float3 Color    : COLOR;
    float2 UV       : UV;
};

struct VertexOutput
{
    float4 Position : SV_Position;
    float3 Color    : COLOR;
    float2 UV       : UV;
};

struct FragmentInput
{
    float3 Color    : LOCATION0;
    float2 TexCoord : LOCATION1;
};

[shader("vertex")]
VertexOutput main(VertexInput input)
{
    var worldPosition = mul(ubo.model, float4(input.Position, 1.0));
    var viewPosition  = mul(ubo.view, worldPosition);
    var clipPosition  = mul(ubo.proj, viewPosition);

    VertexOutput output;

    output.Color     = input.Color;
    output.UV        = input.UV;
    output.Position  = clipPosition;

    return output;
}

[shader("fragment")]
float4 main(FragmentInput input) : SV_Target
{
    return texSampler.Sample(input.TexCoord);
}
