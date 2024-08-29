namespace ThirdParty.Shaderc;

public struct IncludeResult
{
    public unsafe struct Native
    {
        public byte*  SourceName;
        public size_t SourceNameLength;
        public byte*  Content;
        public size_t ContentLength;
        public void*  UserData;
    }

    public string SourceName { get; set; }
    public byte[] Content    { get; set; }
}
