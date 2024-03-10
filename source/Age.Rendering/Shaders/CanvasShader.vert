#version 450

layout(location = 0) in vec2 inPosition;

layout(location = 0) out vec2 outFragTexCoord;

layout(push_constant) uniform Data
{
    vec2  ViewportSize;
    vec2  Size;
    vec2  Position;
    vec2  UV[4];
    float Color[4];
} data;

float normalize2(float value)
{
    return value * 2 - 1;
}

float denormalize2(float value)
{
    return value / 2 + 0.5;
}

vec2 normalize2(vec2 value)
{
    return vec2(normalize2(value.x), normalize2(value.y));
}

vec2 denormalize2(vec2 value)
{
    return vec2(denormalize2(value.x), denormalize2(value.y));
}

float applyScale(float value, float scale)
{
    float result = 0;

    if (scale > 0)
    {
        value = denormalize2(value);

        result = normalize2(value - value * (1 - scale));
    }

    return result;
}

vec2 applyScale(in vec2 point, in vec2 scale)
{
    return vec2(applyScale(point.x, scale.x), applyScale(point.y, scale.y));
}

void main()
{
    vec2 offset = data.Position / data.ViewportSize * vec2(2, -2);
    vec2 scale  = data.Size     / data.ViewportSize;

    vec2 position = applyScale(inPosition, scale);

    outFragTexCoord = data.UV[gl_VertexIndex];

    gl_Position = vec4(position + offset, 0, 1);
}
