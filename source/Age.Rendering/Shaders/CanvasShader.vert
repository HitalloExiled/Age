#version 450

#define PI 3.1415926535897932384626433832795

layout(location = 0) in vec2 inPosition;

layout(location = 0) out vec2 outFragTexCoord;

#include "./CanvasShader.PushConstant.glsl"

void main()
{
    vec2 vertex = inPosition / 2 + 0.5;
    vertex.y *= -1;
    vertex *= data.rect.size;
    vertex += data.rect.position;
    vertex =  data.transform.rotation * vertex;
    vertex += data.transform.position;
    vertex.y *= -1;
    vertex /= data.viewport;
    vertex = vertex * 2 - 1;

    outFragTexCoord = data.uv[gl_VertexIndex];
    gl_Position     = vec4(vertex, 1, 1);
}
