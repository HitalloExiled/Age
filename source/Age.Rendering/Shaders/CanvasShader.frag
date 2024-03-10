#version 450

layout(binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

layout(push_constant) uniform Data
{
    vec2  ViewportSize;
    vec2  Size;
    vec2  Position;
    vec2  UV[4];
    float Color[4];
} data;

void main() {
    vec4 color = texture(texSampler, inFragTexCoord);

    outColor = vec4(1 - color.xyz, color.w) * vec4(data.Color[0], data.Color[1], data.Color[2], data.Color[3]);
    // outColor = color;
}
