using System.IO.Compression;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Task = Microsoft.Build.Utilities.Task;

namespace Build.Tasks;

internal delegate void Log(string message, params object[] messageArgs);

public class Logger(TaskLoggingHelper? helper = null)
{
    private readonly Log logMessage = helper != null ? helper.LogMessage : Console.WriteLine;
    private readonly Log logError   = helper != null ? helper.LogError : Console.WriteLine;
    private readonly Log logWarning = helper != null ? helper.LogWarning : Console.WriteLine;

    public void LogMessage(string message, params object[] messageArgs) =>
        this.logMessage.Invoke(message, messageArgs);

    public void LogError(string message, params object[] messageArgs) =>
        this.logError.Invoke(message, messageArgs);

    public void LogWarning(string message, params object[] messageArgs) =>
        this.logWarning.Invoke(message, messageArgs);

    public static implicit operator Logger(TaskLoggingHelper helper) => new(helper);
}

public class DownloadFileUnzip : Task
{
    public static class Executor
    {
        public static async Task<bool> ExecuteAsync(string url, string destination, string[]? files = null, Logger? logger = null)
        {
            logger ??= new();

            try
            {
                using var client = new HttpClient();

                logger.LogMessage($"Downloading '{url}'...");

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Failed to download '{url}'. Reason: {response.ReasonPhrase}");

                    return false;
                }

                var zip = Path.GetTempFileName();

                await using var responseStream = await response.Content.ReadAsStreamAsync();

                await using var fileStream = File.OpenWrite(zip);

                await responseStream.CopyToAsync(fileStream);

                fileStream.Close();

                Directory.CreateDirectory(destination);

                if (files == null)
                {
                    logger.LogMessage($"Extracting to '{destination}'...");

                    ZipFile.ExtractToDirectory(zip, destination, true);
                }
                else
                {
                    using var archive = ZipFile.OpenRead(zip);

                    foreach (var file in files)
                    {
                        var entry = archive.GetEntry(file);

                        if (entry == null)
                        {
                            logger.LogWarning($"file {file} dont exists");

                            continue;
                        }

                        logger.LogMessage($"Extracting '{entry.Name}'...");

                        entry.ExtractToFile(Path.Join(destination, entry.Name), true);
                    }
                }

                logger.LogMessage("Done!");

                return true;
            }
            catch (InvalidDataException)
            {
                logger.LogError($"The downloaded file from '{url}' is not a valid ZIP archive");

                return false;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while downloading or extracting from '{url}': {ex.Message}");

                return false;
            }
        }
    }

    [Required]
    public string Url { get; set; } = null!;

    [Required]
    public string Destination { get; set; } = null!;

    public string[]? Files { get; set; }

    private void LogHigh(string message) =>
        this.Log.LogMessage(MessageImportance.High, message);

    private void LogError(string message) =>
        this.Log.LogError(message);

    public override bool Execute() =>
        Executor.ExecuteAsync(this.Url, this.Destination, this.Files, this.Log).Result;
}
