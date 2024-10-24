#version 450

#include "./CanvasShader.PushConstant.glsl"

layout(binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

void main() {
    // outColor = data.color;
    outColor = vec4(1, 0, 0, 1);
}
