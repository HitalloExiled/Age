// #version 450

// layout(binding = 0)
// uniform UniformBufferObject {
//     mat4 model;
//     mat4 view;
//     mat4 proj;
// } ubo;

// layout(location = 0)
// in vec3 inPosition;

// layout(location = 1)
// in vec3 inColor;

// layout(location = 2)
// in vec2 inTexCoord;

// layout(location = 0)
// out vec3 fragColor;

// layout(location = 1)
// out vec2 fragTexCoord;

// void main() {
//     mat4 viewMatrix    = ubo.view;
//     vec4 worldPosition = ubo.model * vec4(inPosition, 1.0);
//     vec4 viewPosition  = viewMatrix * worldPosition;

//     gl_Position = ubo.proj * viewPosition;

//     fragColor    = inColor;
//     fragTexCoord = inTexCoord;
// }
#version 450
layout(column_major) uniform;
layout(column_major) buffer;

#line 1349 0
struct _MatrixStorage_float4x4std140_0
{
    vec4 data_0[4];
    // mat4 data_0;
};


#line 5 1
struct UniformBufferObject_std140_0
{
    _MatrixStorage_float4x4std140_0 model_0;
    _MatrixStorage_float4x4std140_0 view_0;
    _MatrixStorage_float4x4std140_0 proj_0;
};


#line 5
struct SLANG_ParameterGroup_ubo_std140_0
{
    UniformBufferObject_std140_0 ubo_0;
};


#line 8
layout(binding = 0)
layout(std140)
uniform UniformBufferObject
{
    UniformBufferObject_std140_0 ubo_0;
} ubo_1;

#line 8
mat4 unpackStorage_0(_MatrixStorage_float4x4std140_0 _S1)
{

#line 8
    return mat4(
        _S1.data_0[0][0], _S1.data_0[0][1], _S1.data_0[0][2], _S1.data_0[0][3],
        _S1.data_0[1][0], _S1.data_0[1][1], _S1.data_0[1][2], _S1.data_0[1][3],
        _S1.data_0[2][0], _S1.data_0[2][1], _S1.data_0[2][2], _S1.data_0[2][3],
        _S1.data_0[3][0], _S1.data_0[3][1], _S1.data_0[3][2], _S1.data_0[3][3]
    );
}


#line 19
layout(location = 0)
out vec3 entryPointParam_main_Color_0;


#line 19
layout(location = 1)
out vec2 entryPointParam_main_UV_0;


#line 19
layout(location = 0)
in vec3 input_Position_0;


#line 19
layout(location = 1)
in vec3 input_Color_0;


#line 19
layout(location = 2)
in vec2 input_UV_0;


#line 26
struct VertexOutput_0
{
    vec4 Position_0;
    vec3 Color_0;
    vec2 UV_0;
};


#line 40
void main()
{
    vec4 clipPosition_0 = vec4(input_Position_0, 1.0) * unpackStorage_0(ubo_1.ubo_0.model_0) * unpackStorage_0(ubo_1.ubo_0.view_0) * unpackStorage_0(ubo_1.ubo_0.proj_0);
    // vec4 clipPosition_0 = unpackStorage_0(ubo_1.ubo_0.proj_0) * unpackStorage_0(ubo_1.ubo_0.view_0) * unpackStorage_0(ubo_1.ubo_0.model_0) * vec4(input_Position_0, 1.0);

    // vec4 viewPosition  = (unpackStorage_0(ubo_1.ubo_0.view_0) * (unpackStorage_0(ubo_1.ubo_0.model_0) * vec4(input_Position_0, 1.0)));


    VertexOutput_0 output_0;

    output_0.Color_0 = input_Color_0;
    output_0.UV_0 = input_UV_0;
    output_0.Position_0 = clipPosition_0;

    VertexOutput_0 _S2 = output_0;

#line 52
    gl_Position = output_0.Position_0;

#line 52
    entryPointParam_main_Color_0 = _S2.Color_0;

#line 52
    entryPointParam_main_UV_0 = _S2.UV_0;

#line 52
}

