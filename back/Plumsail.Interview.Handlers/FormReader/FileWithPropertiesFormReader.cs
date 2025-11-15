using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using Plumsail.Interview.Handlers.Extensions;

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Plumsail.Interview.Handlers.FormReader;

/// <summary>
/// Manual parser of POCO object with optional<file> in it
/// </summary>
/// <typeparam name="T">POCO object type</typeparam>
public abstract partial class FileWithPropertiesFormReader<T> where T : new()
{
    public async IAsyncEnumerable<T> ReadAsync(
        Stream body,
        string contentType,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var currentFileRecords = new Dictionary<int, T>();

        if (!MultipartExtensions.IsMultipartContentType(contentType))
        {
            throw new InvalidOperationException("Expected a multipart request");
        }

        const int defaultMultipartBoundaryLengthLimit = 128;
        var boundary = MultipartExtensions.GetBoundary(
            MediaTypeHeaderValue.Parse(contentType),
            defaultMultipartBoundaryLengthLimit);

        var reader = new MultipartReader(boundary, body);

        while (await reader.ReadNextSectionAsync(cancellationToken) is { } section)
        {
            if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
            {
                continue;
            }

            var fieldName = contentDisposition.Name.Value;
            if (string.IsNullOrEmpty(fieldName))
            {
                continue;
            }

            // Parse field name like "files[0].File", "files[0].Description", "[0].FirstName", "[0].Tags[0]"
            var match = UploadedFilesRegex().Match(fieldName);
            if (!match.Success)
            {
                continue;
            }

            var index = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var propertyName = match.Groups[2].Value;
            var arrayIndex = match.Groups[3].Success ? int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture) : (int?)null;

            if (!currentFileRecords.TryGetValue(index, out var record))
            {
                record = new T();
                currentFileRecords[index] = record;
            }

            if (MultipartExtensions.HasFileContentDisposition(contentDisposition))
            {
                // section.Body gets disposed when moving to next section, so we must copy it
                // Use temporary file to avoid buffering large files into memory (LOH)
                var fileName = HeaderUtilities.RemoveQuotes(contentDisposition.FileName).Value ?? string.Empty;
                var sectionContentType = section.ContentType ?? "application/octet-stream";

                var tempFilePath = Path.GetTempFileName();
                var fileSize = await section.Body.CopyToTempFileAsync(tempFilePath, cancellationToken);
                var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 81920, FileOptions.Asynchronous | FileOptions.DeleteOnClose);

                SetFileData(ref record, fileName, sectionContentType, fileSize, fileStream);
            }
            else
            {
                // This is a form field (Description, Status, Priority, etc.)
                using var streamReader = new StreamReader(section.Body, Encoding.UTF8, leaveOpen: false);
                var value = await streamReader.ReadToEndAsync(cancellationToken);

                SetProperty(ref record, propertyName, value, arrayIndex);
            }

            currentFileRecords[index] = record;
        }

        // Process all collected file records and yield them
        foreach (var entry in currentFileRecords.Select(kvp => kvp.Value))
        {
            yield return entry;
        }
    }

    protected abstract void SetProperty(ref T form, string propertyName, string value, int? arrayIndex);

    protected abstract void SetFileData(ref T record, string fileName, string contentType, long size, Stream stream);

    [GeneratedRegex(@"(?:files)?\[(\d+)\]\.(\w+)(?:\[(\d+)\])?")]
    private static partial Regex UploadedFilesRegex();
}