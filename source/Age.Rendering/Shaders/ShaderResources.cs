using System.Diagnostics;
using System.Security.Cryptography;
using Age.Core;
using Age.Rendering.Interfaces;
using ThirdParty.Shaderc;
using ThirdParty.Shaderc.Enums;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Shaders;


public abstract class ShaderResources : IDisposable
{
    private const int DEBOUNCE_DELAY = 50;

    public event Action? Changed;

    private static readonly string            shadersPath = Path.GetFullPath(Debugger.IsAttached ? Path.Join(Directory.GetCurrentDirectory(), "source/Age.Rendering/Shaders") : Path.Join(AppContext.BaseDirectory, $"Shaders"));
    private static readonly FileSystemWatcher watcher     = new(shadersPath) { EnableRaisingEvents = true };

    private readonly Dictionary<string, CancellationTokenSource>    cancellationTokenSources = [];
    private readonly HashSet<string>                                files                    = [];
    private readonly Dictionary<string, byte[]>                     hashes                   = [];
    private readonly Dictionary<string, List<string>>               includeShaders           = [];
    private readonly Dictionary<string, Dictionary<string, byte[]>> shaderIncludes           = [];
    private readonly Dictionary<string, object>                     locks                    = [];

    private bool disposed;

    public abstract string              Name              { get; }
    public abstract VkPrimitiveTopology PrimitiveTopology { get; }

    public Dictionary<VkShaderStageFlags, byte[]> Stages { get; } = [];

    protected ShaderResources(string[] files)
    {
        foreach (var file in files)
        {
            var (absolute, relative) = Path.IsPathRooted(file)
                ? (file, Path.GetRelativePath(shadersPath, file))
                : (Path.GetFullPath(Path.Join(shadersPath, file)), file);

            if (!watcher.Filters.Contains(relative))
            {
                watcher.Filters.Add(relative);
            }

            this.cancellationTokenSources[absolute] = new();
            this.shaderIncludes[absolute] = [];
            this.locks[absolute] = new();

            this.CompileShader(absolute);
            this.files.Add(absolute);
        }

        watcher.Changed += this.OnFileChanged;
    }

    private void CompileShaderWithDebounce(string filepath, string? trigger)
    {
        var cancellationTokenSource = this.cancellationTokenSources[filepath];

        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();

        void action(Task task)
        {
            if (task.IsCompletedSuccessfully && this.CompileShader(filepath, trigger))
            {
                this.Changed?.Invoke();
            }
        }

        Task.Delay(DEBOUNCE_DELAY, cancellationTokenSource.Token)
            .ContinueWith(action, TaskScheduler.Default);
    }

    private bool CompileShader(string filepath, string? trigger = null)
    {
        var padlock = this.locks[filepath];

        lock (padlock)
        {
            var (shaderStage, shaderKind) = Path.GetExtension(filepath) switch
            {
                ".frag" => (VkShaderStageFlags.Fragment, ShaderKind.FragmentShader),
                ".vert" => (VkShaderStageFlags.Vertex,   ShaderKind.VertexShader),
                _ => throw new InvalidOperationException()
            };

            try
            {
                var source   = File.ReadAllBytes(filepath);
                var checksum = MD5.HashData(source);

                var canCompile = trigger != null
                    ? !this.shaderIncludes[filepath].TryGetValue(trigger, out var includeHash) || !includeHash.AsSpan().SequenceEqual(MD5.HashData(File.ReadAllBytes(trigger)))
                    : !this.hashes.TryGetValue(filepath, out var shaderHash) || !shaderHash.AsSpan().SequenceEqual(checksum);

                if (canCompile)
                {
                    this.hashes[filepath] = checksum;

                    Logger.Trace($"Compiling Shader {this.Name}.{shaderStage} [{filepath}]");

                    foreach (var include in this.shaderIncludes[filepath])
                    {
                        watcher.Filters.Remove(Path.GetRelativePath(shadersPath, include.Key));
                    }

                    foreach (var includeShader in this.includeShaders.Values)
                    {
                        includeShader.Remove(filepath);
                    }

                    this.shaderIncludes[filepath].Clear();

                    using var compiler = new Compiler();
                    using var options  = new CompilerOptions { IncludeResolver = this.GetIncludeResolver(filepath) };

                    var result = compiler.CompileIntoSpv(source, shaderKind, filepath, "main", options);

                    foreach (var include in this.shaderIncludes[filepath])
                    {
                        var relativeIncludePath = Path.GetRelativePath(shadersPath, include.Key);

                        if (!watcher.Filters.Contains(relativeIncludePath))
                        {
                            watcher.Filters.Add(relativeIncludePath);
                        }
                    }

                    if (result.CompilationStatus == CompilationStatus.Success)
                    {
                        this.Stages[shaderStage] = result.Bytes;

                        Logger.Trace($"Shader {this.Name}.{shaderStage} [{filepath}] compiled with {(result.Warnings > 0 ? $"{result.Warnings} warnings" : "success")}.");

                        return true;
                    }
                    else
                    {
                        Logger.Error(result.ErrorMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
            }

            return false;
        }
    }

    private IncludeResolve GetIncludeResolver(string shaderfile) =>
        (requestedSource, type, requestingSource, includeDepth) =>
        {
            var filepath = type == IncludeType.Relative
                ? Path.GetFullPath(Path.Join(Path.GetDirectoryName(requestingSource), requestedSource))
                : requestedSource;

            if (!this.includeShaders.TryGetValue(filepath, out var shaders))
            {
                this.includeShaders[filepath] = shaders = [];
            }

            shaders.Add(shaderfile);

            var content           = File.ReadAllBytes(filepath);
            this.hashes[filepath] = MD5.HashData(content);

            this.shaderIncludes[shaderfile][filepath] = MD5.HashData(content);

            return new()
            {
                SourceName = filepath,
                Content    = content,
            };
        };

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var filepath = Path.GetFullPath(e.FullPath);

        if (this.files.Contains(filepath))
        {
            this.CompileShaderWithDebounce(filepath, null);
        }
        else if (this.includeShaders.TryGetValue(filepath, out var files))
        {
            foreach (var file in files)
            {
                this.CompileShaderWithDebounce(file, filepath);
            }
        }
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            foreach (var file in this.files)
            {
                watcher.Filters.Remove(file);
            }

            watcher.Changed -= this.OnFileChanged;

            this.disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}

public abstract class ShaderResources<TVertexInput, TPushConstant>(string[] files) : ShaderResources(files)
where TVertexInput : IVertexInput
where TPushConstant : IPushConstant;
