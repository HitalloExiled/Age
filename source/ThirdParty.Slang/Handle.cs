namespace ThirdParty.Slang;

internal record struct Handle<T>(nint Value)
{
    public override readonly string ToString() =>
        "0x" + this.Value.ToString("x");
}
