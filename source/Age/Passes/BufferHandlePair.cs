using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Passes;

public record struct BufferHandlePair(nint Handle, Buffer Buffer);
