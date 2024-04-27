#version 450

layout(binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

layout(push_constant) uniform Data
{
    vec2  viewport;
    mat2  rotation;
    vec2  position;
    vec2  size;
    vec2  offset;
    vec2  uv[4];
    float color[4];
    uint  border_size;
    uint  border_radius;
    float border_color[4];
    uint  border_position;
} data;

void main() {
    outColor = vec4(data.color[0], data.color[1], data.color[2], data.color[3]);
}
