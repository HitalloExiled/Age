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

#define FLAGS_GRAYSCALE_TEXTURE   1 << 0
#define FLAGS_MULTIPLY_COLOR      1 << 1
#define FLAGS_COLOR_AS_BACKGROUND 1 << 2

layout(binding = 0) uniform sampler2D diffuse;

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
    return uint(min(radius, min(data.rect.size.x * 0.5, data.rect.size.y * 0.5)));
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
            float start = left_top.position.x  + int(position.y) / left_top.size.y   * left_top.size.x;
            float end   = top_right.position.x + int(position.y) / -top_right.size.y * top_right.size.x + top_right.size.x;

            return position.x < start || position.x > end;
        }
        case BORDER_RIGHT:
        {
            float start = top_right.position.y    + int(data.rect.size.x - position.x) / top_right.size.x     * top_right.size.y;
            float end   = right_bottom.position.y + int(data.rect.size.x - position.x) / -right_bottom.size.x * right_bottom.size.y + right_bottom.size.y;

            return position.y < start || position.y > end;
        }
        case BORDER_BOTTOM:
        {
            float start = (bottom_left.position.x  + int(data.rect.size.y - position.y) / bottom_left.size.y   * bottom_left.size.x);
            float end   = (right_bottom.position.x + int(data.rect.size.y - position.y) / -right_bottom.size.y * right_bottom.size.x + right_bottom.size.x);

            return position.x < start || position.x > end;
        }
        case BORDER_LEFT:
        {
            float start = left_top.position.y    + int(position.x) / left_top.size.x     * left_top.size.y;
            float end   = bottom_left.position.y + int(position.x) / -bottom_left.size.x * bottom_left.size.y + bottom_left.size.y;

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

    vec2  origin;
    float radius;
    float thickness_x;
    float thickness_y;

    switch (corner)
    {
        case CORNER_LEFT_TOP:
            {
                radius    = get_min_radius(data.border.radius.left_top);
                origin    = vec2(radius);
                thickness_x = data.border.left.thickness;
                thickness_y = data.border.top.thickness;

                float range = left_top.position.x + position.y / left_top.size.y * left_top.size.x;

                color = position.x > range ? to_vec4(data.border.top.color) : to_vec4(data.border.left.color);
            }
            break;
        case CORNER_TOP_RIGHT:
            {
                radius    = get_min_radius(data.border.radius.top_right);
                origin    = vec2(data.rect.size.x - radius, radius);
                thickness_x = data.border.right.thickness;
                thickness_y = data.border.top.thickness;

                float range = top_right.position.x + position.y / -top_right.size.y * top_right.size.x + top_right.size.x;

                color = position.x < range ? to_vec4(data.border.top.color) : to_vec4(data.border.right.color);
            }
            break;
        case CORNER_RIGHT_BOTTOM:
            {
                radius      = get_min_radius(data.border.radius.right_bottom);
                origin      = vec2(data.rect.size.x - radius, data.rect.size.y - radius);
                thickness_x = data.border.right.thickness;
                thickness_y = data.border.bottom.thickness;

                float range = right_bottom.position.x + (data.rect.size.y - position.y) / -right_bottom.size.y * right_bottom.size.x + right_bottom.size.x;

                color = position.x < range ? to_vec4(data.border.bottom.color) : to_vec4(data.border.right.color);
            }
            break;
        case CORNER_BOTTOM_LEFT:
        {
            radius    = get_min_radius(data.border.radius.bottom_left);
            origin    = vec2(radius, data.rect.size.y - radius);
            thickness_x = data.border.left.thickness;
            thickness_y = data.border.bottom.thickness;

            float range = bottom_left.position.x + (data.rect.size.y - position.y) / bottom_left.size.y * bottom_left.size.x;

            color = position.x > range ? to_vec4(data.border.bottom.color) : to_vec4(data.border.left.color);
        }
        break;
    }

    float direction_length = length(position - origin);

    if (!(is_outside = direction_length > radius))
    {
        if (thickness_x == thickness_y)
        {
            return direction_length > radius - thickness_x;
        }

        float radius_x = radius > thickness_x ? radius - thickness_x : 0;
        float radius_y = radius > thickness_y ? radius - thickness_y : 0;

        if (radius_x > 0 && radius_y > 0)
        {
            return !is_point_inside_ellipse(position, vec2(origin), radius_x, radius_y);
        }

        return true;
    }

    return false;
}

#endif
