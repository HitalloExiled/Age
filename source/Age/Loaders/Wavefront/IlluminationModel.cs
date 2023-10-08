namespace Age.Loaders.Wavefront;

public enum IlluminationModel
{
    ColorOnAndAmbientOff,
    ColorOnAndAmbientOn,
    HighlightOn,
    ReflectionOnAndRayTraceOn,
    TransparencyGlassOnReflectionRayTraceOn,
    ReflectionFresnelOnAndRayTraceOn,
    TransparencyRefractionOnReflectionFresnelOffAndRayTraceOn,
    TransparencyRefractionOnReflectionFresnelOnAndRayTraceOn,
    ReflectionOnAndRayTraceOff,
    TransparencyGlassOnReflectionRayTraceOff,
    CastsShadowsOntoInvisibleSurfaces,
}
