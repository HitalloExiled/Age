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
