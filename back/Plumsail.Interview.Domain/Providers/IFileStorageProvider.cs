using Plumsail.Interview.Domain.Models;

namespace Plumsail.Interview.Domain.Providers;

/// <summary>
/// S3 like file providers
/// </summary>
public interface IFileStorageProvider
{
    /// <summary>
    /// Save multiple files inside storage
    /// </summary>
    /// <param name="fileRecords">Enumerable fileRecords</param>
    /// <param name="cancellationToken">CTS</param>
    /// <returns>ValueTask</returns>
    ValueTask SaveFilesAsync(IEnumerable<FileRecord> fileRecords, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get single file metadata + stream by id
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="cancellationToken">CTS</param>
    /// <returns>ValueTask with FileRecord</returns>
    ValueTask<FileRecord> GetFileByIdAsync(Guid fileId, CancellationToken cancellationToken = default);
}