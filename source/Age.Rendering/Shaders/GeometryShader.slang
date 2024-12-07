struct UniformBufferObject
{
    float4x4 model;
    float4x4 view;
    float4x4 proj;
};

cbuffer ubo : register(b0)
{
    UniformBufferObject uboData;
}

struct VertexInput
{
    float3 inPosition : LOCATION0;
    float3 inColor    : LOCATION1;
    float2 inTexCoord : LOCATION2;
};

struct VertexOutput
{
    float4 position     : SV_Position;
    float3 fragColor    : LOCATION0;
    float2 fragTexCoord : LOCATION1;
};

struct FragmentInput
{
    float3 Color    : LOCATION0;
    float2 TexCoord : LOCATION1;
};

struct FragmentOutput
{
    float4 Color : SV_Target;
};

Texture2D    texSampler      : register(t1);
SamplerState texSamplerState : register(s1);

[shader("vertex")]
VertexOutput main(VertexInput input)
{
    var worldPosition = mul(uboData.model, float4(input.inPosition, 1.0));
    var viewPosition  = mul(uboData.view, worldPosition);
    var clipPosition  = mul(uboData.proj, viewPosition);

    VertexOutput output;

    output.fragColor    = input.inColor;
    output.fragTexCoord = input.inTexCoord;
    output.position     = clipPosition;

    return output;
}

[shader("fragment")]
FragmentOutput main(FragmentInput input)
{
    FragmentOutput output;

    output.Color = texSampler.Sample(texSamplerState, input.TexCoord);

    return output;
}
