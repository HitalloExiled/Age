#version 450

#include "./CanvasShader.PushConstant.glsl"

layout(binding = 0) uniform sampler2D diffuse;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

void main() {
    vec4 color = texture(diffuse, inFragTexCoord);
    outColor = vec4(color.rrr, color.g);
}
