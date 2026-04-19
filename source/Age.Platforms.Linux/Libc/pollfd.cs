namespace Age.Platforms.Linux.Libc;

internal struct pollfd
{
    public int fd;
    public short events;
    public short revents;
}
