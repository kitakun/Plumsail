using Mediator;

using Plumsail.Interview.DatabaseContext;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;
using Plumsail.Interview.Domain.Providers;

namespace Plumsail.Interview.Handlers.SubmissionHandlers;

public readonly record struct FileUploadData(
    string FileName,
    long Size,
    string ContentType,
    Stream Stream,
    string? Description,
    SubmissionStatusEnum? Status,
    DateTime? CreatedDate,
    PriorityLevelEnum? Priority,
    bool? IsPublic);

public sealed record SubmissionsPutRequest(
    IAsyncEnumerable<FileUploadData> FileUploadData
) : IRequest<OperationResult<Dictionary<string, Guid>>>;

public sealed class SubmissionsPutHandler(
    IFileStorageProvider fileStorageProvider,
    IIdentityProvider identityProvider,
    PlumsailDbContext dbContext)
    : IRequestHandler<SubmissionsPutRequest, OperationResult<Dictionary<string, Guid>>>
{
    public async ValueTask<OperationResult<Dictionary<string, Guid>>> Handle(SubmissionsPutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fileRecords = new List<FileRecord>();
            var fileDictionary = new Dictionary<string, Guid>();

            await foreach (var file in request.FileUploadData.WithCancellation(cancellationToken))
            {
                var fileId = identityProvider.GenerateId();

                var fileRecord = new FileRecord
                {
                    Id = fileId,
                    Name = file.FileName,
                    Size = file.Size,
                    Type = file.ContentType,
                    Stream = file.Stream
                };

                var submissionEntity = new SubmissionEntity
                {
                    Id = fileId,
                    Name = file.FileName,
                    Size = file.Size,
                    Type = file.ContentType,
                    Description = file.Description,
                    Status = file.Status,
                    CreatedDate = file.CreatedDate ?? DateTime.UtcNow,
                    Priority = file.Priority,
                    IsPublic = file.IsPublic
                };

                fileRecords.Add(fileRecord);
                dbContext.Submissions.Add(submissionEntity);
                fileDictionary[file.FileName] = fileId;
            }

            await fileStorageProvider.SaveFilesAsync(fileRecords, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return OperationResult<Dictionary<string, Guid>>.Success(fileDictionary);
        }
        catch (Exception ex)
        {
            return OperationResult<Dictionary<string, Guid>>.Fail(ex);
        }
    }
}