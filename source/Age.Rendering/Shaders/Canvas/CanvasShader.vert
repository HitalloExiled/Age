#version 450

layout(location = 0) in vec2 inPosition;

layout(location = 0) out vec2 outFragTexCoord;

#include "./CanvasShader.PushConstant.glsl"

void main()
{
    mat3 matrix = transform_to_mat3(data.transform);

    vec2 vertex = inPosition / 2 + 0.5;
    vertex.y *= -1;
    vertex *= data.rect.size;
    vertex += data.rect.position;
    vertex = (matrix * vec3(vertex, 1)).xy;
    vertex.y *= -1;
    vertex /= data.viewport;
    vertex = vertex * 2 - 1;

    outFragTexCoord = data.uv[gl_VertexIndex];
    gl_Position     = vec4(vertex, 1, 1);
}
