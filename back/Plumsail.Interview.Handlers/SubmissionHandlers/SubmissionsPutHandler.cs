using Mediator;

using System.Text.Json;

using Plumsail.Interview.DatabaseContext.Services;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;
using Plumsail.Interview.Domain.Providers;

namespace Plumsail.Interview.Handlers.SubmissionHandlers;

public readonly record struct FileUploadData(
    string FileName,
    long Size,
    string ContentType,
    Stream Stream,
    Dictionary<string, JsonElement>? Payload = null);

public sealed record SubmissionsPutRequest(
    IAsyncEnumerable<FileUploadData> FileUploadData
) : IRequest<OperationResult<Dictionary<string, Guid>>>;

public sealed class SubmissionsPutHandler(
    IFileStorageProvider fileStorageProvider,
    IIdentityProvider identityProvider,
    ISubmissionDataService submissionDataService)
    : IRequestHandler<SubmissionsPutRequest, OperationResult<Dictionary<string, Guid>>>
{
    public async ValueTask<OperationResult<Dictionary<string, Guid>>> Handle(SubmissionsPutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fileRecords = new List<FileRecord>();
            var fileDictionary = new Dictionary<string, Guid>();
            var submissionEntities = new List<SubmissionEntity>();

            await foreach (var file in request.FileUploadData.WithCancellation(cancellationToken))
            {
                var fileId = identityProvider.GenerateId();

                var payload = file.Payload ?? new Dictionary<string, JsonElement>();

                // Ensure CreatedDate is always set
                if (!payload.ContainsKey("CreatedDate"))
                {
                    payload["CreatedDate"] = JsonSerializer.SerializeToElement(
                        DateTime.UtcNow,
                        HandlersJsonSerializerContext.Default.DateTime);
                }

                var payloadJson = JsonSerializer.SerializeToElement(payload, HandlersJsonSerializerContext.Default.DictionaryStringJsonElement);

                // Generate a filename if none is provided (for form data without file uploads)
                var fileName = !string.IsNullOrEmpty(file.FileName)
                    ? file.FileName
                    : $"form-data-{fileId}";
                var contentType = !string.IsNullOrEmpty(file.ContentType)
                    ? file.ContentType
                    : "application/json";

                var fileRecord = new FileRecord
                {
                    Id = fileId,
                    FileData = new FileData(fileName, file.Size, contentType),
                    Payload = payloadJson,
                    Stream = file.Stream
                };

                var submissionEntity = new SubmissionEntity
                {
                    Id = fileId,
                    FileData = new FileData(fileName, file.Size, contentType),
                    Payload = payloadJson
                };

                fileRecords.Add(fileRecord);
                submissionEntities.Add(submissionEntity);
                fileDictionary[fileName] = fileId;
            }

            await fileStorageProvider.SaveFilesAsync(fileRecords, cancellationToken);

            await submissionDataService.InsertAsync(submissionEntities, cancellationToken);

            return OperationResult<Dictionary<string, Guid>>.Success(fileDictionary);
        }
        catch (Exception ex)
        {
            return OperationResult<Dictionary<string, Guid>>.Fail(ex);
        }
    }
}