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
    public event Action? Changed;

    private static readonly string            shadersPath = Debugger.IsAttached ? Path.Join(Directory.GetCurrentDirectory(), "source/Age.Rendering/Shaders") : Path.Join(AppContext.BaseDirectory, $"Shaders");
    private static readonly FileSystemWatcher watcher     = new(shadersPath) { EnableRaisingEvents = true };

    private readonly HashSet<string>            files;
    private readonly Dictionary<string, byte[]> hashes = [];

    private CancellationTokenSource? cancellationTokenSource;
    private bool                     disposed;

    public abstract string              Name              { get; }
    public abstract VkPrimitiveTopology PrimitiveTopology { get; }

    public Dictionary<VkShaderStageFlags, byte[]> Stages { get; } = [];

    protected ShaderResources(HashSet<string> files)
    {
        foreach (var file in files)
        {
            if (!watcher.Filters.Contains(file))
            {
                watcher.Filters.Add(file);
            }

            this.CompileShader(file);
        }

        watcher.Changed += this.OnFileChanged;

        this.files = files;
    }

    private bool CompileShader(string filename)
    {
        var (shaderStage, shaderKind) = Path.GetExtension(filename) switch
        {
            ".frag" => (VkShaderStageFlags.Fragment, ShaderKind.FragmentShader),
            ".vert" => (VkShaderStageFlags.Vertex,   ShaderKind.VertexShader),
            _ => throw new InvalidOperationException()
        };

        try
        {
            var fileStream   = new FileStream(Path.Join(shadersPath, filename), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var binaryReader = new BinaryReader(fileStream);
            var source       = binaryReader.ReadBytes((int)fileStream.Length);

            var checksum = MD5.HashData(source);

            if (!this.hashes.TryGetValue(filename, out var oldhash) || !oldhash.AsSpan().SequenceEqual(checksum))
            {
                this.hashes[filename] = checksum;

                Logger.Trace($"Compiling Shader {filename}");

                fileStream.Dispose();
                binaryReader.Dispose();

                using var compiler = new Compiler();

                var result = compiler.CompileIntoSpv(source, shaderKind, Path.GetFileName(filename), "main");

                if (result.CompilationStatus == CompilationStatus.Success)
                {
                    this.Stages[shaderStage] = result.Bytes;

                    Logger.Trace($"Shader {filename} compiled with {(result.Warnings > 0 ? $"{result.Warnings} warnings" : "success")}.");

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

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (!this.files.Contains(e.Name!))
        {
            return;
        }

        this.cancellationTokenSource?.Cancel();

        this.cancellationTokenSource = new();

        void action(Task task)
        {
            if (task.IsCompletedSuccessfully && this.CompileShader(e.Name!))
            {
                this.Changed?.Invoke();
            }
        }

        Task.Delay(50, this.cancellationTokenSource.Token).ContinueWith(action, TaskScheduler.Default);
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

public abstract class ShaderResources<TVertexInput, TPushConstant>(HashSet<string> files) : ShaderResources(files)
where TVertexInput : IVertexInput
where TPushConstant : IPushConstant;
