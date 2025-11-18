using Mediator;

using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

using Plumsail.Interview.Domain.Models;
using Plumsail.Interview.Handlers.FormReader;
using Plumsail.Interview.Handlers.SubmissionHandlers;
using Plumsail.Interview.Web.Attributes;
using Plumsail.Interview.Web.Extensions;

using System.Text.Json;

namespace Plumsail.Interview.Web.Controllers;

public static class SubmissionController
{
    public static RouteGroupBuilder MapSubmissionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/submission")
            .WithMetadata(
                new ApiControllerAttribute(),
                new ProducesAttribute("application/json"),
                new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest),
                new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent));

        group.MapGet("/",
                async Task<IResult> (
                    IMediator mediator,
                    [FromQuery] int? offset,
                    [FromQuery] int? limit,
                    CancellationToken cancellationToken) =>
                {
                    var request = new SubmissionsGetRequest(offset, limit);
                    var response = await mediator.Send(request, cancellationToken);
                    return Results.Ok(response);
                })
            .Produces<OperationResult<Pagination<FileRecord>>>()
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces<object>(StatusCodes.Status400BadRequest)
            .WithMetadata(
                new HttpGetAttribute(),
                new ProducesResponseTypeAttribute(typeof(OperationResult<Pagination<FileRecord>>), StatusCodes.Status200OK));
        
        group.MapGet("search",
                async (
                    IMediator mediator,
                    [FromQuery] string? searchTerm,
                    [FromQuery] int? offset,
                    [FromQuery] int? limit,
                    CancellationToken cancellationToken) =>
                {
                    var request = new SubmissionsSearchRequest(searchTerm, offset, limit);
                    var response = await mediator.Send(request, cancellationToken);
        
                    if (!response.IsSuccess)
                    {
                        return Results.BadRequest(response.Exception?.Message ?? "Failed to search files");
                    }
        
                    return Results.Ok(response);
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces<object>(StatusCodes.Status400BadRequest)
            .WithMetadata(
                new HttpGetAttribute("search"),
                new ProducesResponseTypeAttribute(typeof(OperationResult<Pagination<FileRecord>>), StatusCodes.Status200OK));
        
        group.MapPut("/",
                async (
                    IMediator mediator,
                    [FromServices] FileWithPropertiesFormReader<FileUploadData> formReader,
                    HttpRequest request,
                    CancellationToken cancellationToken) =>
                {
                    try
                    {
                        IAsyncEnumerable<FileUploadData> fileUploadData;
                        var contentType = request.ContentType ?? string.Empty;

                        if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                        {
                            fileUploadData = FormExtensions.CreateSingleJsonObjectAsyncEnumerator(request.Body, cancellationToken);
                        }
                        else
                        {
                            fileUploadData = formReader.CreateAsyncEnumerator(
                                request.Body,
                                contentType,
                                cancellationToken);
                        }

                        var putRequest = new SubmissionsPutRequest(fileUploadData);
                        var response = await mediator.Send(putRequest, cancellationToken);

                        if (!response.IsSuccess)
                        {
                            return Results.BadRequest(response.Exception?.Message ?? "Failed to upload files");
                        }

                        return Results.Ok(response);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return Results.BadRequest(ex.Message);
                    }
                    catch (JsonException ex)
                    {
                        return Results.BadRequest($"Invalid JSON: {ex.Message}");
                    }
                })
            .Produces<OperationResult<Dictionary<string, Guid>>>()
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces<object>(StatusCodes.Status400BadRequest)
            .WithMetadata(
                new HttpPutAttribute(),
                new AcceptsMetadata(["multipart/form-data", "application/json"], typeof(JsonElement)),
                new DisableFormValueModelBindingAttribute(),
                new DisableRequestSizeLimitAttribute()
            );
        
        group.MapGet("pre-sign",
                async (
                    IMediator mediator,
                    [FromQuery] string token,
                    CancellationToken cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        return Results.BadRequest("Token is required");
                    }
        
                    var request = new SubmissionGetByPreSignRequest(token);
                    var response = await mediator.Send(request, cancellationToken);
        
                    if (!response.IsSuccess)
                    {
                        if (response.Exception is FileNotFoundException)
                        {
                            return Results.NotFound(response.Exception.Message);
                        }
        
                        return Results.BadRequest(response.Exception?.Message ?? "Failed to get file");
                    }
        
                    if (response.Result?.Stream == null)
                    {
                        return Results.NotFound("File stream not found");
                    }
        
                    return Results.File(
                        response.Result.Stream,
                        response.Result.FileData.Type,
                        response.Result.FileData.Name);
                })
            .Produces<object>()
            .Produces<object>(StatusCodes.Status400BadRequest)
            .Produces<object>(StatusCodes.Status204NoContent)
            .WithMetadata(
                new HttpGetAttribute("pre-sign"),
                new ProducesResponseTypeAttribute(typeof(FileStreamResult), StatusCodes.Status200OK));

        return group;
    }
}