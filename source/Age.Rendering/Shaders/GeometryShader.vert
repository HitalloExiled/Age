#version 450

layout(binding = 0) uniform UniformBufferObject {
    mat4 model;
    mat4 view;
    mat4 proj;
} ubo;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inColor;
layout(location = 2) in vec2 inTexCoord;

layout(location = 0) out vec3 fragColor;
layout(location = 1) out vec2 fragTexCoord;

void main() {
    mat4 model = mat4(
        vec4(0.7071067, 0, 0.7071068, 0),
        vec4(0, 1, 0, 0),
        vec4(-0.7071068, 0, 0.7071067, 0),
        vec4(0, -2, 0, 1)
    );
    // model[3] = vec4(0, 0, 0, 1);

    mat4 viewMatrix    = ubo.view;
    vec4 worldPosition = ubo.model * vec4(inPosition, 1.0);
    vec4 viewPosition  = viewMatrix * worldPosition;

    gl_Position = ubo.proj * viewPosition;

    fragColor    = inColor;
    fragTexCoord = inTexCoord;
}
