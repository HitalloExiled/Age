namespace ThirdParty.Slang;

internal record struct Handle<T>(nint Value) where T : ManagedSlang<T>
{
    public override readonly string ToString() =>
        "0x" + this.Value.ToString("x");
}
