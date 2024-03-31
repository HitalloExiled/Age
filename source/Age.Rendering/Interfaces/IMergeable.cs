namespace Age.Rendering.Interfaces;

public interface IMergeable<T> where T : IMergeable<T>
{
    abstract static T Merge(T left, T right);
    abstract static void Update(T source, T target);
    T Merge(T other);
    void Update(T source);
}
