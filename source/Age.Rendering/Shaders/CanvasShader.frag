#version 450

#define CORNER_LEFT_TOP     1
#define CORNER_TOP_RIGHT    2
#define CORNER_RIGHT_BOTTOM 3
#define CORNER_BOTTOM_LEFT  4

#define BORDER_TOP    1
#define BORDER_RIGHT  2
#define BORDER_BOTTOM 3
#define BORDER_LEFT   4

layout(binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

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

layout(push_constant) uniform Data
{
    vec2      viewport;
    Transform transform;
    Rect      rect;
    vec2      uv[4];
    float     color[4];
    bool      grayscale;
    Border    border;
} data;

vec4 to_vec4(float elements[4])
{
    return vec4(elements[0], elements[1], elements[2], elements[3]);
}

bool has_flag(uint value, uint flag)
{
    return (value & flag) == flag;
}

bool intersects(vec2 position, Rect rect)
{
    return position.x >= rect.position.x && position.x <= rect.position.x + rect.size.x && position.y >= rect.position.y && position.y <= rect.position.y + rect.size.y;
}

vec2 cubic_bezier(vec2 p0, vec2 p1, vec2 p2, vec2 p3, float t) {
    mat4 M = mat4(
         1.0,  0.0,  0.0, 0.0,
        -3.0,  3.0,  0.0, 0.0,
         3.0, -6.0,  3.0, 0.0,
        -1.0,  3.0, -3.0, 1.0
    );

    vec4 T = vec4(1.0, t, t * t, t * t * t);

    vec4 result = M * T;

    float x = dot(result, vec4(p0.x, p1.x, p2.x, p3.x));
    float y = dot(result, vec4(p0.y, p1.y, p2.y, p3.y));

    return vec2(x, y);
}

vec2 quadratic_bezier(vec2 p0, vec2 p1, vec2 p2, float t)
{
    vec2 q0 = mix(p0, p1, t);
    vec2 q1 = mix(p1, p2, t);

    return mix(q0, q1, t);
}

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
            float start = left_top.position.x + position.y / left_top.size.y * left_top.size.x;
            float end   = top_right.position.x + position.y / -top_right.size.y * top_right.size.x + top_right.size.x;

            return position.x < start || position.x > end;
        }
        case BORDER_RIGHT:
        {
            float start = top_right.position.y    + (data.rect.size.x - position.x) / top_right.size.x     * top_right.size.y;
            float end   = right_bottom.position.y + (data.rect.size.x - position.x) / -right_bottom.size.x * right_bottom.size.y + right_bottom.size.y;

            return position.y < start || position.y > end;
        }
        case BORDER_BOTTOM:
        {
            float start = bottom_left.position.x  + (data.rect.size.y - position.y) / bottom_left.size.y * bottom_left.size.x;
            float end   = right_bottom.position.x + (data.rect.size.y - position.y) / -right_bottom.size.y * right_bottom.size.x + right_bottom.size.x;

            return position.x < start || position.x > end;
        }
        case BORDER_LEFT:
        {
            float start = left_top.position.y    + position.x / left_top.size.x     * left_top.size.y;
            float end   = bottom_left.position.y + position.x / -bottom_left.size.x * bottom_left.size.y + bottom_left.size.y;

            return position.y < start || position.y > end;
        }
    }

    return false;
}

bool is_border(vec2 position, out BorderSide side)
{
    if (position.y >= 0 && position.y < data.border.top.thickness && !intersects_borders(position, BORDER_TOP))
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
    else if (position.x >= 0 && position.x < data.border.left.thickness && !intersects_borders(position, BORDER_LEFT))
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
    float min_side = min(data.rect.size.x, data.rect.size.y);

    uint left_top_radius     = uint(min(data.border.radius.left_top,     min_side));
    uint top_right_radius    = uint(min(data.border.radius.top_right,    min_side));
    uint right_bottom_radius = uint(min(data.border.radius.right_bottom, min_side));
    uint bottom_left_radius  = uint(min(data.border.radius.bottom_left,  min_side));

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
    switch (corner)
    {
        case CORNER_LEFT_TOP:
        {
            uint radius    = get_min_radius(data.border.radius.left_top);
            vec2 direction = position - vec2(radius);

            float factor = dot(normalize(direction), vec2(-1, 0));

            color = mix(to_vec4(data.border.top.color), to_vec4(data.border.left.color), factor);

            if (!(is_outside = length(direction) > radius))
            {
                vec2 p1 = vec2(radius,                     data.border.top.thickness);
                vec2 p2 = vec2(data.border.left.thickness, data.border.top.thickness);
                vec2 p3 = vec2(data.border.left.thickness, radius);

                vec2 curve = quadratic_bezier(p1, p2, p3, (position.y - data.border.top.thickness) / (radius - data.border.top.thickness));

                return position.x < curve.x;

                // float influence = 1 - (position.y - data.border.top.thickness) / (radius - data.border.top.thickness);
                // float curve = data.border.left.thickness + (radius - data.border.left.thickness) * pow(influence, 3);

                // return position.x < curve;
            }

            return false;

        }
        case CORNER_TOP_RIGHT:
        {
            uint radius    = get_min_radius(data.border.radius.top_right);
            vec2 direction = position - vec2(data.rect.size.x - radius, radius);

            float factor = dot(normalize(direction), vec2(1, 0));

            color = mix(to_vec4(data.border.top.color), to_vec4(data.border.right.color), factor);

            if (!(is_outside = length(direction) > radius))
            {
                vec2 p1 = vec2(data.rect.size.x - radius,                      data.border.top.thickness);
                vec2 p2 = vec2(data.rect.size.x - data.border.right.thickness, data.border.top.thickness);
                vec2 p3 = vec2(data.rect.size.x - data.border.right.thickness, radius);

                vec2 curve = quadratic_bezier(p1, p2, p3, (position.y - data.border.top.thickness) / (radius - data.border.top.thickness));

                return position.x > curve.x;
            }

            return false;
        }
        case CORNER_RIGHT_BOTTOM:
        {
            uint radius    = get_min_radius(data.border.radius.right_bottom);
            vec2 direction = position - vec2(data.rect.size.x - radius, data.rect.size.y - radius);

            float factor = dot(normalize(direction), vec2(1, 0));

            color = mix(to_vec4(data.border.bottom.color), to_vec4(data.border.right.color), factor);

            if (!(is_outside = length(direction) > radius))
            {
                vec2 p1 = vec2(data.rect.size.x - radius,                      data.rect.size.y - data.border.bottom.thickness);
                vec2 p2 = vec2(data.rect.size.x - data.border.right.thickness, data.rect.size.y - data.border.bottom.thickness);
                vec2 p3 = vec2(data.rect.size.x - data.border.right.thickness, data.rect.size.y - radius);

                vec2 curve = quadratic_bezier(p1, p2, p3, (data.rect.size.y - position.y - data.border.bottom.thickness) / (radius - data.border.bottom.thickness));

                return position.x > curve.x;
            }

            return false;
        }
        case CORNER_BOTTOM_LEFT:
        {
            uint radius    = get_min_radius(data.border.radius.bottom_left);
            vec2 direction = position - vec2(radius, data.rect.size.y - radius);

            float factor = dot(normalize(direction), vec2(-1, 0));

            color = mix(to_vec4(data.border.bottom.color), to_vec4(data.border.left.color), factor);

            if (!(is_outside = length(direction) > radius))
            {
                vec2 p1 = vec2(radius,                     data.rect.size.y - data.border.bottom.thickness);
                vec2 p2 = vec2(data.border.left.thickness, data.rect.size.y - data.border.bottom.thickness);
                vec2 p3 = vec2(data.border.left.thickness, data.rect.size.y - radius);

                vec2 curve = quadratic_bezier(p1, p2, p3, (data.rect.size.y - position.y - data.border.bottom.thickness) / (radius - data.border.bottom.thickness));

                return position.x < curve.x;
            }

            return false;
        }
    }

    return false;
}

void main()
{
    vec4 texture_color = texture(texSampler, inFragTexCoord);

    vec4 color = data.grayscale
        ? vec4(1 - texture_color.rrr, texture_color.g) * to_vec4(data.color)
        : to_vec4(float[](data.border.top.color[0], data.border.top.color[1], data.border.top.color[2], 0.2));

    bool has_border = data.border.top.thickness
        + data.border.right.thickness
        + data.border.bottom.thickness
        + data.border.left.thickness
        + data.border.radius.left_top
        + data.border.radius.top_right
        + data.border.radius.right_bottom
        + data.border.radius.bottom_left > 0;

    if (has_border)
    {
        vec2 position = data.rect.size * inFragTexCoord;

        BorderSide side;
        uint corner;

        if (is_corner(position, corner))
        {
            vec4 corner_color;
            bool is_outside;
            if (is_inside_radius(position, corner, corner_color, is_outside))
            {
                color = corner_color;
            }
            else if (is_outside)
            {
                // color = vec4(0);
                color = vec4(0, 1, 1, 1);
            }
            else
            {
                color = vec4(1, 0, 1, 1);
            }
        }
        else if (is_border(position, side))
        {
            color = to_vec4(side.color);
        }
    }

    outColor = color;
}
