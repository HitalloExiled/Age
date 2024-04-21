#version 450

#define PI 3.1415926535897932384626433832795

layout(location = 0) in vec2 inPosition;

layout(location = 0) out vec2 outFragTexCoord;

layout(push_constant) uniform Data
{
    vec2  viewport;
    mat2  rotation;
    vec2  position;
    vec2  size;
    vec2  offset;
    vec2  uv[4];
    float color[4];
} data;

void main()
{
    vec2 vertex = inPosition / 2 + 0.5;
    vertex.y *= -1;
    vertex *= data.size;
    vertex += data.offset;
    vertex = data.rotation * vertex;
    vertex += data.position;
    vertex.y *= -1;
    vertex /= data.viewport;
    vertex = vertex * 2 - 1;

    outFragTexCoord = data.uv[gl_VertexIndex];
    gl_Position     = vec4(vertex, 1, 1);
}
