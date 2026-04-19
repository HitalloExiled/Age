namespace ThirdParty.Wayland;

public interface IWaylandInterface<TManaged, TUmanaged>
where TManaged  : IWaylandInterface<TManaged, TUmanaged>
where TUmanaged : unmanaged
{
    internal static abstract TManaged Create(Handle<TUmanaged> handle);
}
