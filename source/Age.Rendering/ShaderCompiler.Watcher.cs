using Age.Core;
using Age.Core.Extensions;
using Age.Rendering.Resources;

namespace Age.Rendering;

public partial class ShaderCompiler
{
    private class Watcher : IDisposable
    {
        public event FileSystemEventHandler Changed
        {
            add    => this.fileSystemWatcher.Changed += value;
            remove => this.fileSystemWatcher.Changed -= value;
        }

        private bool disposed;

        private readonly FileSystemWatcher fileSystemWatcher = new(Shader.ShadersPath)
        {
            EnableRaisingEvents = true,
            IncludeSubdirectories = true,
        };

        public Dictionary<string, HashSet<string>>                 Dependencies  { get; } = [];
        public Dictionary<string, FileEntry>                       Files         { get; } = [];
        public Dictionary<string, Dictionary<Shader, ShaderEntry>> ShaderEntries { get; } = [];

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.fileSystemWatcher.Dispose();

                    foreach (var entries in this.ShaderEntries.Values)
                    {
                        foreach (var entry in entries.Values)
                        {
                            entry.Dispose();
                        }
                    }
                }

                this.disposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Watch(Shader shader, in ShaderOptions shaderOptions)
        {
            ref var entries = ref this.ShaderEntries.GetValueRefOrAddDefault(shader.Filepath, out var exists);

            if (!exists)
            {
                entries = [];

                this.fileSystemWatcher.Filters.Add(Path.GetRelativePath(Shader.ShadersPath, shader.Filepath));
            }

            entries!.Add(shader, new(shader, shaderOptions));
            shader.Disposed += () => this.Unwatch(shader);
        }

        public void Watch(string dependecy, Shader dependent)
        {
            ref var dependents = ref this.Dependencies.GetValueRefOrAddDefault(dependecy, out var exists);

            if (!exists)
            {
                dependents = [];

                this.fileSystemWatcher.Filters.Add(Path.GetRelativePath(Shader.ShadersPath, dependecy));
            }

            dependents!.Add(dependent.Filepath);
        }

        public void Unwatch(Shader shader)
        {
            if (this.ShaderEntries.TryGetValue(shader.Filepath, out var entries) && entries.Remove(shader, out var entry))
            {
                entry.Dispose();

                this.fileSystemWatcher.Filters.Remove(Path.GetRelativePath(Shader.ShadersPath, shader.Filepath));

                foreach (var dependent in this.Dependencies.Values)
                {
                    dependent.Remove(shader.Filepath);
                }
            }
        }

        public void Unwatch(string dependecy, Shader dependent, out bool clear)
        {
            clear = false;

            if (this.Dependencies.TryGetValue(dependecy, out var dependents))
            {
                dependents.Remove(dependent.Filepath);

                if (dependents.Count == 0)
                {
                    this.Dependencies.Remove(dependecy);
                    this.fileSystemWatcher.Filters.Remove(Path.GetRelativePath(Shader.ShadersPath, dependecy));

                    clear = true;
                }
            }
        }

        public void Update(Shader shader, ReadOnlySpan<string> dependencies)
        {
            using var diff = shader.Dependencies.AsSpan().Diff(dependencies);

            foreach (var dependecy in dependencies)
            {
                ref var fileEntry = ref this.Files.GetValueRefOrAddDefault(dependecy, out var exists);

                using var bytes = FileReader.ReadAllBytes(dependecy);

                if (!exists)
                {
                    fileEntry = new(MD5Hash.Create(bytes));
                }
                else
                {
                    MD5Hash.Update(bytes, ref fileEntry.Hash);
                }
            }

            foreach (var dependecy in diff.Added)
            {
                this.Watch(dependecy, shader);
            }

            foreach (var dependecy in diff.Removed)
            {
                this.Unwatch(dependecy, shader, out var clear);

                if (clear)
                {
                    this.Files.Remove(dependecy);
                }
            }

            shader.Dependencies.ReplaceRange(0.., dependencies);
        }
    }
}
