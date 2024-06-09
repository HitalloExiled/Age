#ifndef COMMON
#define COMMON

#define PI 3.1415926535897931

struct Rect
{
    vec2 size;
    vec2 position;
};

struct Transform
{
    mat2 rotation;
    vec2 position;
};

struct Line
{
    vec2 start;
    vec2 end;
};

#endif
