namespace Age.Graphs;

[Serializable]
public class RenderGraphCiclicException : Exception
{
    public RenderGraphCiclicException() { }
    public RenderGraphCiclicException(string message) : base(message) { }
    public RenderGraphCiclicException(string message, Exception inner) : base(message, inner) { }
}
