namespace Age.Platforms.Linux.LibDecor;

internal unsafe struct libdecor_interface
{
    public required delegate* unmanaged<
        libdecor*      /* context */,
        libdecor_error /* error */,
        byte*          /* message */,
        void
    > error;

    public void* reserved0;
	public void* reserved1;
	public void* reserved2;
	public void* reserved3;
	public void* reserved4;
	public void* reserved5;
	public void* reserved6;
	public void* reserved7;
	public void* reserved8;
	public void* reserved9;
}
