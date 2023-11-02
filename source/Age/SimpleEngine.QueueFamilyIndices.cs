namespace Age;

public unsafe partial class SimpleEngine
{
    public record QueueFamilyIndices
    {
        public uint? GraphicsFamily { get; set; }
        public uint? PresentFamily  { get; set; }

        public bool IsComplete => this.GraphicsFamily.HasValue && this.PresentFamily.HasValue;
    }
}
