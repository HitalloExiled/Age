// #version 450

// layout(binding = 1)
// uniform sampler2D texSampler;

// layout(location = 0)
// in vec3 fragColor;

// layout(location = 1)
// in vec2 fragTexCoord;

// layout(location = 0)
// out vec4 outColor;

// void main() {
//     outColor = texture(texSampler, fragTexCoord);
// }
#version 450
layout(column_major) uniform;
layout(column_major) buffer;

#line 17 0
layout(binding = 1)
uniform sampler2D texSampler_0;


#line 4811 1
layout(location = 0)
out vec4 entryPointParam_main_0;


#line 4811
layout(location = 1)
in vec2 input_TexCoord_0;


#line 56 0
void main()
{

#line 56
    entryPointParam_main_0 = (texture((texSampler_0), (input_TexCoord_0)));

#line 56
    return;
}

