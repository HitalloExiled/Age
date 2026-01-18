using Age.Core;
using Age.Rendering.Resources;

namespace Age.Rendering;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable - BUGGY

public partial class ShaderCompiler
{
    private struct FileEntry(MD5Hash hash) : IDisposable
    {
        private CancellationTokenSource cancellationTokenSource = new();

        public MD5Hash Hash = hash;

        private readonly void DisposeToken()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
        }

        public CancellationToken RefreshToken()
        {
            this.DisposeToken();

            this.cancellationTokenSource = new();

            return this.cancellationTokenSource.Token;
        }

        public readonly void Dispose() =>
            this.DisposeToken();
    }

    private struct ShaderEntry(Shader shader, ShaderOptions shaderOptions) : IDisposable
    {
        private CancellationTokenSource cancellationTokenSource = new();

        public readonly Lock   Lock   { get; }               = new();
        public readonly Shader Shader { get; }               = shader;
        public readonly ShaderOptions ShaderOptions { get; } = shaderOptions;

        public Task? Execution { get; set; }

        private readonly void DisposeToken()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
        }

        public CancellationToken RefreshToken()
        {
            this.DisposeToken();

            this.cancellationTokenSource = new();

            return this.cancellationTokenSource.Token;
        }

        public readonly void Dispose() =>
            this.DisposeToken();
    }
}
