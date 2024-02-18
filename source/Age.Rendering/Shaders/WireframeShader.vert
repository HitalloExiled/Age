#version 450

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec2 inFragTexCoord;

layout(location = 0) out vec3 fragColor;
layout(location = 1) out vec2 fragTexCoord;

void main() {
    gl_Position  = vec4(inPosition, 1);
    fragTexCoord = inFragTexCoord;
}
