#ifndef CANVAS_SHADER_PUSH_CONSTANT
#define CANVAS_SHADER_PUSH_CONSTANT

#include "../Includes/Rect.glsl"
#include "../Includes/Transform.glsl"

struct BorderSide
{
    uint  thickness;
    float color[4];
};

struct BorderRadius
{
    uint left_top;
    uint top_right;
    uint right_bottom;
    uint bottom_left;
};

struct Border
{
    BorderSide   top;
    BorderSide   right;
    BorderSide   bottom;
    BorderSide   left;
    BorderRadius radius;
};

struct Line
{
    vec2 start;
    vec2 end;
};

layout(push_constant, std430) uniform Data
{
    // [16-bytes boundary]
    vec4 color;

    // [8-bytes boundary]
    vec2      viewport;
    Transform transform;
    Rect      rect;
    vec2      uv[4];
    Border    border;

    // [4-bytes boundary]
    uint flags;
} data;

#endif
