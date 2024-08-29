namespace ThirdParty.SpirvCross;

public record ReflectedResource
{
    public required uint   BaseTypeId { get; init; }
    public required uint   Id         { get; init; }
    public required string Name       { get; init; }
    public required uint   TypeId     { get; init; }
}
