namespace Age.Scenes;

public abstract partial class Node
{
    public readonly ref struct UnsealedScope
    {
        private readonly Node node;

        public UnsealedScope(Node node)
        {
            node.Unseal();

            this.node = node;
        }

        public readonly void Dispose() => this.node.Seal();
    }
}
