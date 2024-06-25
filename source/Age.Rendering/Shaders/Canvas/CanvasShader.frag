#version 450

#include "../Includes/Common.glsl"
#include "./CanvasBase.glsl"

void main()
{
    vec4 texture_color = texture(texSampler, inFragTexCoord);

    vec4 color =
        has_flag(data.flags, FLAGS_COLOR_AS_BACKGROUND)
            ? data.color
            : has_flag(data.flags, FLAGS_GRAYSCALE_TEXTURE | FLAGS_MULTIPLY_COLOR)
                ? vec4(1 - texture_color.rrr, texture_color.g) * data.color
                : texture_color;

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
                discard;
            }
        }
        else if (is_border(position, side))
        {
            color = to_vec4(side.color);
        }
    }


    outColor = color;
    // outColor = color * vec4(1, 0, 1, 1);
}
