#include "./Geometry3DShader.slang"

[shader("fragment")]
float4 main(FragmentInput input) : SV_Target
{
    return sampledTexture.Sample(input.uv);
}
