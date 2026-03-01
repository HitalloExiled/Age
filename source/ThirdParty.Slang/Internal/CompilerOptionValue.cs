namespace ThirdParty.Slang.Internal;

public unsafe struct CompilerOptionValue
{
    public CompilerOptionValueKind Kind;
    public int32_t                 IntValue0;
    public int32_t                 IntValue1;
    public byte*                   StringValue0;
    public byte*                   StringValue1;
}
