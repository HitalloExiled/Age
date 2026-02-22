namespace Age.Core;

public sealed class ListPool<T> : CollectionPool<List<T>, T>
{
    protected override List<T> Create() => [];
}
