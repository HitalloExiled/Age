namespace Age.Core;

public partial struct UnsafeLock
{
    private const int LOCKED   = 1;
    private const int UNLOCKED = 0;

    private volatile int state;

    public static Scope Lock(ref UnsafeLock unsafeLock)
    {
        unsafeLock.Lock();

        return new(ref unsafeLock);
    }

    public void Lock()
    {
        var spinner = new SpinWait();

        while (Interlocked.CompareExchange(ref this.state, LOCKED, UNLOCKED) != UNLOCKED)
        {
            spinner.SpinOnce();
        }
    }

    public void Unlock()
    {
        if (Interlocked.CompareExchange(ref this.state, UNLOCKED, LOCKED) != LOCKED)
        {
            throw new UnsafeLockException("Trying to unlock a lock that was not locked.");
        }
    }
}
