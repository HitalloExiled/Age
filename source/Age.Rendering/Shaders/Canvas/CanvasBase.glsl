#ifndef CANVAS_COMMON
#define CANVAS_COMMON

#include "../Includes/Common.glsl"
#include "./CanvasShader.PushConstant.glsl"

#define CORNER_LEFT_TOP     1
#define CORNER_TOP_RIGHT    2
#define CORNER_RIGHT_BOTTOM 3
#define CORNER_BOTTOM_LEFT  4

#define BORDER_TOP    1
#define BORDER_RIGHT  2
#define BORDER_BOTTOM 3
#define BORDER_LEFT   4

#define CURVE_FACTOR 3

#define FLAGS_GRAYSCALE_TEXTURE     1 << 0
#define FLAGS_MULTIPLY_COLOR        1 << 1
#define FLAGS_COLOR_AS_BACKGROUND   1 << 2

layout(binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

void get_corners(out Rect left_top, out Rect top_right, out Rect right_bottom, out Rect bottom_left)
{
    left_top     = Rect(vec2(data.border.left.thickness,  data.border.top.thickness),    vec2(0));
    top_right    = Rect(vec2(data.border.right.thickness, data.border.top.thickness),    vec2(data.rect.size.x - data.border.right.thickness, 0));
    right_bottom = Rect(vec2(data.border.right.thickness, data.border.bottom.thickness), vec2(data.rect.size.x - data.border.right.thickness, data.rect.size.y - data.border.bottom.thickness));
    bottom_left  = Rect(vec2(data.border.left.thickness,  data.border.bottom.thickness), vec2(0, data.rect.size.y - data.border.bottom.thickness));
}

uint get_min_radius(uint radius)
{
    return uint(min(radius, min(data.rect.size.x, data.rect.size.y)));
}

vec2 get_max(vec2 vector)
{
    return vec2(max(vector.x, vector.y));
}

bool intersects_borders(vec2 position, uint border)
{
    Rect left_top;
    Rect top_right;
    Rect right_bottom;
    Rect bottom_left;

    get_corners(left_top, top_right, right_bottom, bottom_left);

    switch (border)
    {
        case BORDER_TOP:
        {
            float start = left_top.position.x  + uint(position.y / left_top.size.y)   * left_top.size.x;
            float end   = top_right.position.x + uint(position.y / -top_right.size.y) * top_right.size.x + top_right.size.x;

            return position.x < start || position.x > end;
        }
        case BORDER_RIGHT:
        {
            float start = top_right.position.y    + uint((data.rect.size.x - position.x) / top_right.size.x)     * top_right.size.y;
            float end   = right_bottom.position.y + uint((data.rect.size.x - position.x) / -right_bottom.size.x) * right_bottom.size.y + right_bottom.size.y;

            return position.y < start || position.y > end;
        }
        case BORDER_BOTTOM:
        {
            float start = (bottom_left.position.x  + uint((data.rect.size.y - position.y) / bottom_left.size.y)   * bottom_left.size.x);
            float end   = (right_bottom.position.x + uint((data.rect.size.y - position.y) / -right_bottom.size.y) * right_bottom.size.x + right_bottom.size.x);

            return position.x < start || position.x > end;
        }
        case BORDER_LEFT:
        {
            float start = left_top.position.y    + uint(position.x / left_top.size.x)     * left_top.size.y;
            float end   = bottom_left.position.y + uint(position.x / -bottom_left.size.x) * bottom_left.size.y + bottom_left.size.y;

            return position.y < start || position.y > end;
        }
    }

    return false;
}

bool is_border(vec2 position, out BorderSide side)
{
    if (position.y >= 0 && position.y <= data.border.top.thickness && !intersects_borders(position, BORDER_TOP))
    {
        side = data.border.top;
    }
    else if (position.x >= data.rect.size.x - data.border.right.thickness && !intersects_borders(position, BORDER_RIGHT))
    {
        side = data.border.right;
    }
    else if (position.y >= data.rect.size.y - data.border.bottom.thickness && !intersects_borders(position, BORDER_BOTTOM))
    {
        side = data.border.bottom;
    }
    else if (position.x >= 0 && position.x <= data.border.left.thickness && !intersects_borders(position, BORDER_LEFT))
    {
        side = data.border.left;
    }
    else
    {
        return false;
    }

    return true;
}

bool is_corner(vec2 position, out uint corner)
{
    uint left_top_radius     = get_min_radius(data.border.radius.left_top);
    uint top_right_radius    = get_min_radius(data.border.radius.top_right);
    uint right_bottom_radius = get_min_radius(data.border.radius.right_bottom);
    uint bottom_left_radius  = get_min_radius(data.border.radius.bottom_left);

    if (position.x >= 0 && position.x < left_top_radius && position.y >= 0 && position.y <= left_top_radius)
    {
        corner = CORNER_LEFT_TOP;
    }
    else if (position.y >= 0 && position.y <= top_right_radius && position.x >= data.rect.size.x - top_right_radius)
    {
        corner = CORNER_TOP_RIGHT;
    }
    else if (position.x >= data.rect.size.x - right_bottom_radius && position.y >= data.rect.size.y - right_bottom_radius)
    {
        corner = CORNER_RIGHT_BOTTOM;
    }
    else if (position.y >= data.rect.size.y - bottom_left_radius && position.x >= 0 && position.x < bottom_left_radius)
    {
        corner = CORNER_BOTTOM_LEFT;
    }
    else
    {
        return false;
    }

    return true;
}

bool is_inside_radius(vec2 position, uint corner, out vec4 color, out bool is_outside)
{
    Rect left_top;
    Rect top_right;
    Rect right_bottom;
    Rect bottom_left;

    get_corners(left_top, top_right, right_bottom, bottom_left);

    switch (corner)
    {
        case CORNER_LEFT_TOP:
        {
            uint radius    = get_min_radius(data.border.radius.left_top);
            vec2 direction = position - vec2(radius);

            float top_range = left_top.position.x + position.y / left_top.size.y * left_top.size.x;

            color = position.x > top_range ? to_vec4(data.border.top.color) : to_vec4(data.border.left.color);

            if (!(is_outside = length(direction) > radius))
            {
                if (data.border.top.thickness + data.border.left.thickness == 0)
                {
                    return false;
                }

                if (radius > data.border.top.thickness || radius > data.border.left.thickness)
                {
                    float influence = 1 - (position.y - data.border.top.thickness) / (radius - data.border.top.thickness);
                    float curve = data.border.left.thickness + (radius - data.border.left.thickness) * pow(influence, CURVE_FACTOR);

                    return position.x < curve;
                }

                return true;
            }

        }
        case CORNER_TOP_RIGHT:
        {
            uint radius    = get_min_radius(data.border.radius.top_right);
            vec2 direction = position - vec2(data.rect.size.x - radius, radius);

            float top_range = top_right.position.x + position.y / -top_right.size.y * top_right.size.x + top_right.size.x;

            color = position.x < top_range ? to_vec4(data.border.top.color) : to_vec4(data.border.right.color);

            if (!(is_outside = length(direction) > radius))
            {
                if (data.border.top.thickness + data.border.right.thickness == 0)
                {
                    return false;
                }

                if (radius > data.border.top.thickness || radius > data.border.left.thickness)
                {
                    float influence = 1 - (position.y - data.border.top.thickness) / (radius - data.border.top.thickness);
                    float curve = data.rect.size.x - data.border.right.thickness - (radius - data.border.right.thickness) * pow(influence, CURVE_FACTOR);

                    return position.x > curve;
                }

                return true;
            }
        }
        case CORNER_RIGHT_BOTTOM:
        {
            uint radius    = get_min_radius(data.border.radius.right_bottom);
            vec2 direction = position - vec2(data.rect.size.x - radius, data.rect.size.y - radius);

            float bottom_range = right_bottom.position.x + (data.rect.size.y - position.y) / -right_bottom.size.y * right_bottom.size.x + right_bottom.size.x;

            color = position.x < bottom_range ? to_vec4(data.border.bottom.color) : to_vec4(data.border.right.color);

            if (!(is_outside = length(direction) > radius))
            {
                if (data.border.bottom.thickness + data.border.right.thickness == 0)
                {
                    return false;
                }

                if (radius > data.border.bottom.thickness || radius > data.border.right.thickness)
                {
                    float influence = 1 - (data.rect.size.y - position.y - data.border.bottom.thickness) / (radius - data.border.bottom.thickness);
                    float curve = data.rect.size.x - data.border.right.thickness - (radius - data.border.right.thickness) * pow(influence, CURVE_FACTOR);

                    return position.x > curve;
                }

                return true;
            }
        }
        case CORNER_BOTTOM_LEFT:
        {
            uint radius    = get_min_radius(data.border.radius.bottom_left);
            vec2 direction = position - vec2(radius, data.rect.size.y - radius);

            float bottom_range = bottom_left.position.x + (data.rect.size.y - position.y) / bottom_left.size.y * bottom_left.size.x;

            color = position.x > bottom_range ? to_vec4(data.border.bottom.color) : to_vec4(data.border.left.color);

            if (!(is_outside = length(direction) > radius))
            {
                if (data.border.bottom.thickness + data.border.right.thickness == 0)
                {
                    return false;
                }

                if (radius > data.border.bottom.thickness || radius > data.border.left.thickness)
                {
                    float influence = 1 - (data.rect.size.y - position.y - data.border.bottom.thickness) / (radius - data.border.bottom.thickness);
                    float curve     = data.border.left.thickness + (radius - data.border.left.thickness) * pow(influence, CURVE_FACTOR);

                    return position.x < curve;
                }

                return true;
            }
        }
    }

    return false;
}

#endif
