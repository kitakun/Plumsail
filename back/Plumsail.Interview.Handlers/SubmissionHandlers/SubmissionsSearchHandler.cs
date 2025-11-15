using Mediator;

using Microsoft.EntityFrameworkCore;

using Plumsail.Interview.DatabaseContext;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;

namespace Plumsail.Interview.Handlers.SubmissionHandlers;

public sealed record SubmissionsSearchRequest(
    string? SearchTerm = null,
    int? Offset = null,
    int? Limit = null) : IRequest<OperationResult<Pagination<FileRecord>>>;

public sealed class SubmissionsSearchHandler(
    PlumsailDbContext dbContext,
    IMediator mediator) : IRequestHandler<SubmissionsSearchRequest, OperationResult<Pagination<FileRecord>>>
{
    private static readonly Func<PlumsailDbContext, IAsyncEnumerable<SubmissionEntity>> GetNoTrackingSubmissionsAsync =
        EF.CompileAsyncQuery((PlumsailDbContext context) =>
            context.Submissions.AsNoTracking());

    private static readonly Func<PlumsailDbContext, string, IAsyncEnumerable<SubmissionEntity>> SearchSubmissionsAsync =
        EF.CompileAsyncQuery((PlumsailDbContext context, string searchTerm) =>
            context.Submissions
                .AsNoTracking()
                .Where(s =>
                    s.Name.ToLower().Contains(searchTerm) ||
                    (s.Description != null && s.Description.ToLower().Contains(searchTerm))));

    private static readonly Func<PlumsailDbContext, string, int, int, IAsyncEnumerable<FileRecord>> GetSearchFileRecordsQueryAsync =
        EF.CompileAsyncQuery((PlumsailDbContext context, string searchTerm, int offset, int limit) =>
            context.Submissions
                .AsNoTracking()
                .Where(s =>
                    s.Name.ToLower().Contains(searchTerm) ||
                    (s.Description != null && s.Description.ToLower().Contains(searchTerm)))
                .OrderBy(s => s.CreatedDate)
                .ThenBy(s => s.Id)
                .Skip(offset)
                .Take(limit)
                .Select(s => new FileRecord
                {
                    Id = s.Id,
                    Name = s.Name,
                    Size = s.Size,
                    Type = s.Type,
                    Description = s.Description,
                    Status = s.Status,
                    CreatedDate = s.CreatedDate,
                    Priority = s.Priority,
                    IsPublic = s.IsPublic
                }));

    private static readonly Func<PlumsailDbContext, int, int, IAsyncEnumerable<FileRecord>> GetFileRecordsQueryAsync =
        EF.CompileAsyncQuery((PlumsailDbContext context, int offset, int limit) =>
            context.Submissions
                .AsNoTracking()
                .OrderBy(s => s.CreatedDate)
                .ThenBy(s => s.Id)
                .Skip(offset)
                .Take(limit)
                .Select(s => new FileRecord
                {
                    Id = s.Id,
                    Name = s.Name,
                    Size = s.Size,
                    Type = s.Type,
                    Description = s.Description,
                    Status = s.Status,
                    CreatedDate = s.CreatedDate,
                    Priority = s.Priority,
                    IsPublic = s.IsPublic
                }));

    public async ValueTask<OperationResult<Pagination<FileRecord>>> Handle(SubmissionsSearchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            int totalCount;
            List<FileRecord> fileRecords;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                totalCount = await SearchSubmissionsAsync(dbContext, searchTerm).CountAsync(cancellationToken);
                fileRecords = await GetSearchFileRecordsQueryAsync(
                        dbContext,
                        searchTerm,
                        request.Offset ?? 0,
                        request.Limit ?? totalCount)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                totalCount = await GetNoTrackingSubmissionsAsync(dbContext).CountAsync(cancellationToken);
                fileRecords = await GetFileRecordsQueryAsync(
                        dbContext,
                        request.Offset ?? 0,
                        request.Limit ?? totalCount)
                    .ToListAsync(cancellationToken);
            }

            var preSignTasks = fileRecords.Select(async fileRecord =>
            {
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