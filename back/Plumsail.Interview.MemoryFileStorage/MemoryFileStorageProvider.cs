using System.Collections.Concurrent;

using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;
using Plumsail.Interview.Domain.Providers;

using System.Collections.Immutable;

namespace Plumsail.Interview.MemoryFileStorage;

/// <summary>
/// This FileStorageProvider will store all files in application buffer (RAM)
/// </summary>
public class MemoryFileStorageProvider : IFileStorageProvider
{
    private readonly ConcurrentDictionary<Guid, byte[]> _files = new();

    public async ValueTask SaveFilesAsync(IEnumerable<FileRecord> fileRecords, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var fileRecordsList = fileRecords.ToImmutableList();

        var invalidRecord = fileRecordsList.FirstOrDefault(fileRecord => fileRecord.Id == default);
        if (invalidRecord != null)
        {
            throw new ArgumentException($"File ID cannot be default (empty GUID) for file: {invalidRecord.FileData.Name}", nameof(fileRecords));
        }

        await Parallel.ForEachAsync(fileRecordsList, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, async (fileRecord, ct) =>
        {
            byte[] content;
            if (fileRecord.Stream != null)
            {
                try
                {
                    using var memoryStream = new MemoryStream();
                    await fileRecord.Stream.CopyToAsync(memoryStream, ct);
                    content = memoryStream.ToArray();
                }
                finally
                {
                    try
                    {
                        await fileRecord.Stream.DisposeAsync();
                    }
                    catch
                    {
                        // Ignore disposal errors (e.g., stream already disposed)
                    }
                }
            }
            else
            {
                content = [];
            }

            _files.AddOrUpdate(
                fileRecord.Id,
                content,
                (_, _) => content);
        });
    }

    public ValueTask<FileRecord> GetFileByIdAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_files.TryGetValue(fileId, out var content))
        {
            throw new FileNotFoundException($"File {fileId} not found");
        }

        var fileRecord = new FileRecord
        {
            Id = fileId,
            FileData = new FileData(string.Empty, content.Length, string.Empty),
            Payload = default,
            Stream = new MemoryStream(content)
        };

        return ValueTask.FromResult(fileRecord);
    }
}