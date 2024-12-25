#include "./CanvasShader.Base.slang"

[shader("fragment")]
public FragmentOutput main()
{
    return { data.color };
}
