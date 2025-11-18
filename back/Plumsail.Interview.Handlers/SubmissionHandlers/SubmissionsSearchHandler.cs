using Mediator;

using System.Text.Json;

using Plumsail.Interview.DatabaseContext.Services;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;

namespace Plumsail.Interview.Handlers.SubmissionHandlers;

public sealed record SubmissionsSearchRequest(
    string? SearchTerm = null,
    int? Offset = null,
    int? Limit = null) : IRequest<OperationResult<Pagination<FileRecord>>>;

public sealed class SubmissionsSearchHandler(
    ISubmissionDataService submissionDataService,
    IMediator mediator)
    : IRequestHandler<SubmissionsSearchRequest, OperationResult<Pagination<FileRecord>>>
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

    public async ValueTask<OperationResult<Pagination<FileRecord>>> Handle(SubmissionsSearchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            int totalCount;
            List<SubmissionEntity> submissions;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                var allSubmissions = await submissionDataService.GetAllAsync(cancellationToken);

                submissions = allSubmissions
                    .Where(s =>
                        (s.FileData.HasValue && s.FileData.Value.Name.ToLower().Contains(searchTerm)) ||
                        (s.Payload.ValueKind != JsonValueKind.Undefined &&
                         s.Payload.TryGetProperty("Description", out var desc) &&
                         desc.ValueKind == JsonValueKind.String &&
                         desc.GetString() != null &&
                         desc.GetString()!.ToLower().Contains(searchTerm)))
                    .OrderBy(s =>
                    {
                        if (s.Payload.ValueKind == JsonValueKind.Undefined || !s.Payload.TryGetProperty("CreatedDate", out var cd))
                            return DateTime.MinValue;
                        if (cd.ValueKind == JsonValueKind.String && DateTime.TryParse(cd.GetString(), out var parsedDate))
                            return parsedDate;
                        return DateTime.MinValue;
                    })
                    .ThenBy(s => s.Id)
                    .ToList();

                totalCount = submissions.Count;
                submissions = submissions
                    .Skip(request.Offset ?? 0)
                    .Take(request.Limit ?? totalCount)
                    .ToList();
            }
            else
            {
                var allSubmissions = await submissionDataService.GetAllAsync(cancellationToken);

                totalCount = allSubmissions.Count;
                submissions = allSubmissions
                    .OrderBy(s =>
                    {
                        if (s.Payload.ValueKind == JsonValueKind.Undefined || !s.Payload.TryGetProperty("CreatedDate", out var cd))
                            return DateTime.MinValue;
                        if (cd.ValueKind == JsonValueKind.String && DateTime.TryParse(cd.GetString(), out var parsedDate))
                            return parsedDate;
                        return DateTime.MinValue;
                    })
                    .ThenBy(s => s.Id)
                    .Skip(request.Offset ?? 0)
                    .Take(request.Limit ?? totalCount)
                    .ToList();
            }

            var fileRecords = submissions.Select(MapToFileRecord).ToList();

            var preSignTasks = fileRecords.Select(async fileRecord =>
            {
                if (fileRecord.FileData.Size == 0)
                {
                    return new { FileRecord = fileRecord, PreSignUrl = (string?)null };
                }

                var preSignRequest = new SubmissionGetPreSignRequest(fileRecord.Id);
                var preSignResponse = await mediator.Send(preSignRequest, cancellationToken);
                return new { FileRecord = fileRecord, PreSignUrl = preSignResponse.IsSuccess ? preSignResponse.Result : null };
            });

            var preSignResults = await Task.WhenAll(preSignTasks);

            var fileRecordsWithPreSign = preSignResults.Select(result =>
                result.FileRecord with { PreSign = result.PreSignUrl }
            ).ToList();

            var result = new Pagination<FileRecord>
            {
                Items = fileRecordsWithPreSign,
                TotalCount = totalCount
            };

            return OperationResult<Pagination<FileRecord>>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<Pagination<FileRecord>>.Fail(ex);
        }
    }
}