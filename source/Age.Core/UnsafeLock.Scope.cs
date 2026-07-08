namespace Age.Core;

public partial struct UnsafeLock
{
    public ref struct Scope(ref UnsafeLock unsafeLock) : IDisposable
    {
        private ref UnsafeLock unsafeLock = ref unsafeLock;

        public void Dispose() =>
            this.unsafeLock.Unlock();
    }
}
