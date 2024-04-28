#version 450

layout(binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

layout(push_constant) uniform Data
{
    layout(offset = 80)
    float color[4];
} data;

void main() {
    outColor = vec4(data.color[0], data.color[1], data.color[2], data.color[3]);
}
