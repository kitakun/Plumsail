using Mediator;

using Microsoft.Extensions.Logging;

using Plumsail.Interview.DatabaseContext.Services;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;

namespace Plumsail.Interview.Handlers.SubmissionHandlers;

public sealed record SubmissionsGetRequest(
    int? Offset = null,
    int? Limit = null
) : IRequest<OperationResult<Pagination<FileRecord>>>;

public sealed class SubmissionsGetHandler(
    ISubmissionDataService submissionDataService,
    IMediator mediator,
    ILogger<SubmissionsGetHandler> logger
) : IRequestHandler<SubmissionsGetRequest, OperationResult<Pagination<FileRecord>>>
{
    private static FileRecord MapToFileRecord(SubmissionEntity entity)
    {
        return new FileRecord
        {
            Id = entity.Id,
            FileData = entity.FileData ?? new FileData(string.Empty, 0, string.Empty),
            Payload = entity.Payload
        };
    }

    public async ValueTask<OperationResult<Pagination<FileRecord>>> Handle(SubmissionsGetRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting submissions with offset {Offset} and limit {Limit}", request.Offset, request.Limit);
            var totalCount = await submissionDataService.GetTotalCountAsync(cancellationToken);
            logger.LogInformation("Total count of submissions: {TotalCount}", totalCount);

            var offset = request.Offset ?? 0;
            var limit = request.Limit ?? totalCount;
            logger.LogInformation("Offset: {Offset}, Limit: {Limit}", offset, limit);

            var submissions = await submissionDataService.GetPageAsync(offset, limit, cancellationToken);
            logger.LogInformation("Found {Count} submissions", submissions.Count);
            var fileRecords = submissions.Select(MapToFileRecord).ToList();
            logger.LogInformation("Mapped {Count} file records", fileRecords.Count);

            List<FileRecord> fileRecordsWithPreSign;

            if (fileRecords.Count == 0)
            {
                fileRecordsWithPreSign = fileRecords;
            }
            else
            {
                var preSignTasks = fileRecords.Select(async fileRecord =>
                {
                    if (fileRecord.FileData.Size == 0)
                    {
                        return (FileRecord: fileRecord, PreSignUrl: null);
                    }

                    try
                    {
                        var preSignRequest = new SubmissionGetPreSignRequest(fileRecord.Id);
                        var preSignResponse = await mediator.Send(preSignRequest, cancellationToken);
                        return (
                            FileRecord: fileRecord,
                            PreSignUrl: preSignResponse.IsSuccess
                                ? preSignResponse.Result
                                : null);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to get preSign URL for file {FileId}", fileRecord.Id);
                        return (FileRecord: fileRecord, PreSignUrl: null);
                    }
                });

                var preSignResults = await Task.WhenAll(preSignTasks);

                fileRecordsWithPreSign = preSignResults.Select(result =>
                    result.FileRecord with
                    {
                        PreSign = result.PreSignUrl
                    }
                ).ToList();
            }

            var result = new Pagination<FileRecord>
            {
                Items = fileRecordsWithPreSign,
                TotalCount = totalCount
            };

            return OperationResult<Pagination<FileRecord>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get submissions with offset {Offset} and limit {Limit}", request.Offset, request.Limit);
            return OperationResult<Pagination<FileRecord>>.Fail(ex);
        }
    }
}