namespace Age.Servers.Rendering.Storage;

internal class RendererEnvironmentStorage
{
    private readonly Dictionary<Guid, object> environments = new();
    public bool IsEnvironment(Guid environment) =>
        this.environments.ContainsKey(environment);
}
