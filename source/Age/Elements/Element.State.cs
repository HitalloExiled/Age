namespace Age.Elements;

public abstract partial class Element
{
    private ElementState States
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (this.StyleSheet != null)
                {
                    this.ComputeStyle(this.ComputedStyle.Data);
                }
            }
        }
    }

    internal bool IsScrollable { get; set; }

    private protected void AddState(ElementState state) =>
        this.States |= state;

    private protected void RemoveState(ElementState state) =>
        this.States &= ~state;
}
