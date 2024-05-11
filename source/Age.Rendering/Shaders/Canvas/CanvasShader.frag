#version 450

#include "../Includes/Common.glsl"
#include "./CanvasBase.glsl"

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
