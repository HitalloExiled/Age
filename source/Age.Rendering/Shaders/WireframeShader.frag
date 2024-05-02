#version 450

layout(binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

layout(push_constant, std430) uniform Data
{
    vec4 color;
} data;

void main() {
    outColor = data.color;
}
