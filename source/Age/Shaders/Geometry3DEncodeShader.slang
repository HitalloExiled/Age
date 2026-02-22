#include "./Geometry3DShader.slang"

[vk::push_constant]
PushConstant data;

[shader("fragment")]
float4 main(FragmentInput input) : SV_Target
{
    var color = data.color;

    return data.color;
}
