using System.Collections;
using System.Runtime.CompilerServices;
using Age.Scenes;

namespace Age.Elements.Enumerators;

internal struct ComposedTreeEnumerator(Element target) : IEnumerator<Layoutable>
{
    private readonly Element target = target;
    private Node? current;

    private int state;

    public readonly Layoutable Current => (this.current as Layoutable)!;

    readonly object IEnumerator.Current => this.Current;

    public readonly void Dispose() { }

    public bool MoveNext()
    {
        switch (this.state)
        {
            case 0:
                if (this.target.ShadowRoot != null)
                {
                    this.current = getLayoutableOrSkip(this.target.ShadowRoot.FirstChild);
                    this.state   = 1;
                }
                else
                {
                    this.current = getLayoutableOrSkip(this.target.FirstChild);
                    this.state   = 2;
                }

                break;
            case 1:
                this.current = getLayoutableOrSkip(this.current!.NextSibling);

                break;

            case 2:
                this.current = this.current!.NextSibling;

                break;
        }

        return this.current != null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Layoutable? getLayoutableOrSkip(Node? node)
        {
            while (true)
            {
                if (node?.NextSibling == null)
                {
                    return null;
                }

                node = node.NextSibling;
            }
        }
    }

    public void Reset()
    {
        this.current = null;
        this.state   = 0;
    }
}
