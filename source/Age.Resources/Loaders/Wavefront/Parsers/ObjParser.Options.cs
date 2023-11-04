namespace Age.Resources.Loaders.Wavefront.Parsers;

public partial class ObjParser
{
    public record Options
    {
        public bool SplitByObject { get; init; }
        public bool SplitByGroup  { get; init; }
    }
}
