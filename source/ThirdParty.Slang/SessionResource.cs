namespace ThirdParty.Slang;

public abstract class SessionResource<T> : ManagedSlang<T>
{
    public Session Session { get; }

    private protected SessionResource(Session session, Handle<T> handle) : base(handle) =>
        this.Session = session;
}
