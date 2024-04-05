namespace Age.Rendering.Drawing;

internal struct TrackedValue<T>(T value = default!) where T : notnull
{
    public event Action? Changed;
    private T value = value;

    public T Value
    {
        readonly get => this.value;
        set
        {
            this.Modified = true;

            if (!this.value.Equals(value))
            {
                this.value = value;

                Changed?.Invoke();
            }
        }
    }

    public bool Modified { get; private set; }
}

internal static class TrackedValue
{
    public static TrackedValue<T> GetValue<T>(TrackedValue<T> left, TrackedValue<T> right, T defaultValue = default!) where T : notnull =>
        left.Modified
            ? left
            : right.Modified
                ? right
                : new(defaultValue);

    public static void SetValue<T>(ref TrackedValue<T> field, TrackedValue<T> trackedValue) where T : notnull
    {
        if (trackedValue.Modified)
        {
            field.Value = trackedValue.Value;
        }
    }
}
