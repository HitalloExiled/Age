namespace ThirdParty.Slang;

public enum ResourceAccess : uint
{
    None,
    Read,
    ReadWrite,
    RasterOrdered,
    Append,
    Consume,
    Write,
    Feedback,
    Unknown = 0x7FFFFFFF,
}
