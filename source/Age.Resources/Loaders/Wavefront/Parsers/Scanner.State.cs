namespace Age.Resources.Loaders.Wavefront.Parsers;

public partial class Scanner
{
    private record struct State(int Index, int LineNumber, int LineStart);
}
