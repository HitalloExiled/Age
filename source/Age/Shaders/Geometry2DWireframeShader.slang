#include "./Geometry2DShader.slang"

[shader("fragment")]
public FragmentOutput main()
{
    return { data.color };
}
