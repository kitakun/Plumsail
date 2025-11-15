using Mediator;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Plumsail.Interview.DatabaseContext;
using Plumsail.Interview.Domain.Models;

using System.Security.Cryptography;
using System.Text;

namespace Plumsail.Interview.Handlers.SubmissionHandlers;

public sealed record SubmissionGetPreSignRequest(Guid FileId) : IRequest<OperationResult<string>>;

public sealed class SubmissionGetPreSignHandler(
    PlumsailDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SubmissionGetPreSignRequest, OperationResult<string>>
{
    public async ValueTask<OperationResult<string>> Handle(SubmissionGetPreSignRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var submissionExists = await dbContext.Submissions
                .AsNoTracking()
                .AnyAsync(s => s.Id == request.FileId, cancellationToken);

            if (!submissionExists)
            {
                return OperationResult<string>.Fail(new FileNotFoundException($"File with ID {request.FileId} not found"));
            }

            var token = GeneratePreSignToken(request.FileId);
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return OperationResult<string>.Fail(new InvalidOperationException("HttpContext is not available"));
            }

            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var preSignUrl = $"{baseUrl}/api/submission/pre-sign?token={token}";

            return OperationResult<string>.Success(preSignUrl);
        }
        catch (Exception ex)
        {
            return OperationResult<string>.Fail(ex);
        }
    }

    private static string GeneratePreSignToken(Guid fileId)
    {
        var bytes = fileId.ToByteArray();
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}