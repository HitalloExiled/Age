namespace ThirdParty.FreeDesktop;

internal unsafe struct DBusError
{
    public byte* name;
    public byte* message;

    public uint dummy;

    public void* padding1;
};
