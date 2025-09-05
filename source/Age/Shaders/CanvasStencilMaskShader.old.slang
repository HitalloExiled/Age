#include "./CanvasShader.Base.slang"

[shader("fragment")]
public FragmentOutput main(in FragmentInput input)
{
    var color = texture.Sample(input.uv);

    if (color.r < 0.5)
    {
        discard;
    }

    return { };
}
