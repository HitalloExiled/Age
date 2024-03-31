namespace Age.Rendering.Resources;

public record struct SampledTexture(Texture Texture, Sampler Sampler, UVRect UV = default);
