#version 450

#define BORDER_TYPE_TOP    1
#define BORDER_TYPE_RIGHT  2
#define BORDER_TYPE_BOTTOM 4
#define BORDER_TYPE_LEFT   8

layout(binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec2 inFragTexCoord;

layout(location = 0) out vec4 outColor;

layout(push_constant) uniform Data
{
    vec2  viewport;
    mat2  rotation;
    vec2  position;
    vec2  size;
    vec2  offset;
    vec2  uv[4];
    float color[4];
    bool  grayscale;
    uint  border_size;
    uint  border_radius;
    float border_color[4];
    uint  border_type;
} data;

vec4 to_vec4(float elements[4])
{
    return vec4(elements[0], elements[1], elements[2], elements[3]);
}

bool has_flag(uint value, uint flag)
{
    return (value & flag) == flag;
}

bool is_border(vec2 position, uint border_size, uint border_type)
{
    return has_flag(border_type, BORDER_TYPE_TOP)    && position.y >= 0 && position.y <= border_size
        || has_flag(border_type, BORDER_TYPE_RIGHT)  && position.x >= data.size.x - border_size
        || has_flag(border_type, BORDER_TYPE_BOTTOM) && position.y >= data.size.y - border_size
        || has_flag(border_type, BORDER_TYPE_LEFT)   && position.x >= 0 && position.x < border_size;
}

bool is_corner(vec2 position, uint border_radius, uint border_type)
{
    return has_flag(border_type, BORDER_TYPE_LEFT | BORDER_TYPE_TOP)     && position.x >= 0 && position.x < border_radius  && position.y >= 0 && position.y <= border_radius
        || has_flag(border_type, BORDER_TYPE_TOP | BORDER_TYPE_RIGHT)    && position.y >= 0 && position.y <= border_radius && position.x >= data.size.x - border_radius
        || has_flag(border_type, BORDER_TYPE_RIGHT | BORDER_TYPE_BOTTOM) && position.x >= data.size.x - border_radius && position.y >= data.size.y - border_radius
        || has_flag(border_type, BORDER_TYPE_BOTTOM | BORDER_TYPE_LEFT)  && position.y >= data.size.y - border_radius && position.x >= 0 && position.x < border_radius;
}

bool is_radius(vec2 origin, vec2 position, uint border_size, uint border_radius)
{
    float len = length(position - origin);
    return border_radius >= border_size
        ? len > border_radius - border_size && len <= border_radius
        : len <= border_radius;
}

bool is_radius(vec2 position, uint border_size, uint border_radius, uint border_type)
{
    return is_radius(vec2(border_radius), position, border_size, border_radius)
        || is_radius(vec2(data.size.x - border_radius, border_radius), position, border_size, border_radius)
        || is_radius(vec2(data.size.x - border_radius, data.size.y - border_radius), position, border_size, border_radius)
        || is_radius(vec2(border_radius, data.size.y - border_radius), position, border_size, border_radius);
}

void main()
{
    vec4 texture_color = texture(texSampler, inFragTexCoord);

    vec4 color = data.grayscale
        ? vec4(1 - texture_color.rrr, texture_color.g) * to_vec4(data.color)
        : vec4(0.75, 0.75, 0.75, 1);

    if (data.border_size > 0)
    {
        uint border_size   = 10;
        uint border_radius = 20;
        uint border_type   = 0;

        border_type |= BORDER_TYPE_TOP;
        border_type |= BORDER_TYPE_RIGHT;
        border_type |= BORDER_TYPE_BOTTOM;
        border_type |= BORDER_TYPE_LEFT;

        vec2 position = data.size * inFragTexCoord;

        if (border_radius > 0 && is_corner(position, border_radius, border_type))
        {
            if (is_radius(position, border_size, border_radius, border_type))
            {
                color = to_vec4(data.border_color);
            }
        }
        else if (is_border(position, border_size, border_type))
        {
            color = to_vec4(data.border_color);
        }
    }

    outColor = color;
}
