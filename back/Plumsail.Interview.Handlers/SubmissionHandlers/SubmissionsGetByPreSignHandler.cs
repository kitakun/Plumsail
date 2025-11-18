using Mediator;

using Plumsail.Interview.DatabaseContext.Services;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;
using Plumsail.Interview.Domain.Providers;

namespace Plumsail.Interview.Handlers.SubmissionHandlers;

public sealed record SubmissionGetByPreSignRequest(string Token) : IRequest<OperationResult<FileRecord>>;

public sealed class SubmissionGetByPreSignHandler(
    ISubmissionDataService submissionDataService,
    IFileStorageProvider fileStorageProvider)
    : IRequestHandler<SubmissionGetByPreSignRequest, OperationResult<FileRecord>>
{
    public async ValueTask<OperationResult<FileRecord>> Handle(SubmissionGetByPreSignRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fileId = ExtractFileIdFromToken(request.Token);
            if (fileId == null)
            {
                return OperationResult<FileRecord>.Fail(new UnauthorizedAccessException("Invalid pre-sign token"));
            }

            var submission = await submissionDataService.GetByIdAsync(fileId.Value, cancellationToken);

            if (submission == null)
            {
                return OperationResult<FileRecord>.Fail(new FileNotFoundException($"File with ID {fileId} not found"));
            }

            var fileRecord = await fileStorageProvider.GetFileByIdAsync(fileId.Value, cancellationToken);

            var result = fileRecord with
            {
                FileData = submission.FileData ?? fileRecord.FileData,
                Payload = submission.Payload
            };

            return OperationResult<FileRecord>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<FileRecord>.Fail(ex);
        }
    }

    private static Guid? ExtractFileIdFromToken(string token)
    {
        try
        {
            var normalizedToken = token.Replace("-", "+").Replace("_", "/");
            switch (normalizedToken.Length % 4)
            {
                case 2:
                    normalizedToken += "==";
                    break;
                case 3:
                    normalizedToken += "=";
                    break;
            }

            var bytes = Convert.FromBase64String(normalizedToken);
            return new Guid(bytes);
        }
        catch
        {
            return null;
        }
    }
}