using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.RenderPasses;

public record struct BufferHandlePair(nint Handle, Buffer Buffer);
