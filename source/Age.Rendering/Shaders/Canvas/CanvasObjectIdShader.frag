#version 450

#include "./CanvasBase.glsl"

void main()
{
    vec4 color = data.color;

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
                // color = corner_color;
            }
            else if (is_outside)
            {
                color = vec4(0);
            }
        }
    }

    outColor = color;
}
