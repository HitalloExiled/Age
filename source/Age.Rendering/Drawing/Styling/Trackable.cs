namespace Age.Rendering.Drawing;

public abstract record Trackable<T>
{
    internal abstract event Action? Changed;

    internal virtual T? Parent { get; set; }

    internal static TrackedValue<TValue> GetValue<TValue>(TrackedValue<TValue> left, TrackedValue<TValue>? right) where TValue : notnull =>
        left.Modified
            ? left
            : (right?.Modified ?? false)
                ? right.Value
                : left;

    internal static void SetValue<TValue>(ref TrackedValue<TValue> field, TrackedValue<TValue> trackedValue) where TValue : notnull
    {
        if (trackedValue.Modified)
        {
            field.Value = trackedValue.Value;
        }
    }

    public abstract T Merge(T other);
    public abstract void Update(T source);
}
