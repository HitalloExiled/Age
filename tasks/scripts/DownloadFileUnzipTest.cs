#:project ../Build.Tasks/Build.Tasks.csproj

using Build.Tasks;

await DownloadFileUnzip.Executor.ExecuteAsync("https://github.com/shader-slang/slang/releases/download/v2026.3.1/slang-2026.3.1-windows-x86_64.zip", Path.Join(Directory.GetCurrentDirectory(), ".tmp"), ["bin/slang.dll"]);
