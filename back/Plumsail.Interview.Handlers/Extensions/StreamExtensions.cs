namespace Plumsail.Interview.Handlers.Extensions;

public static class StreamExtensions
{
    public static async Task<long> CopyToTempFileAsync(
        this Stream sourceStream,
        string tempFilePath,
        CancellationToken cancellationToken = default)
    {
        await using var tempFileStream = new FileStream(
            tempFilePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            FileOptions.Asynchronous);
        await sourceStream.CopyToAsync(tempFileStream, cancellationToken);
        return tempFileStream.Length;
    }
}