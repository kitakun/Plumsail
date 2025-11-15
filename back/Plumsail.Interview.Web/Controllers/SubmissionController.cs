using Mediator;

using Microsoft.AspNetCore.Mvc;

using Plumsail.Interview.Domain.Models;
using Plumsail.Interview.Handlers.FormReader;
using Plumsail.Interview.Handlers.SubmissionHandlers;
using Plumsail.Interview.Web.Attributes;

namespace Plumsail.Interview.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubmissionController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get List of all submission with pagination support
    /// </summary>
    /// <param name="offset">How many elements we need to skip</param>
    /// <param name="limit">How many submissions we need to load</param>
    /// <param name="cancellationToken">CTS</param>
    /// <returns>pagination response with FileRecord</returns>
    [HttpGet]
    [ProducesResponseType(typeof(OperationResult<Pagination<FileRecord>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<OperationResult<Pagination<FileRecord>>>> GetListAsync(
        [FromQuery] int? offset = null,
        [FromQuery] int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var request = new SubmissionsGetRequest(offset, limit);
        var response = await mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Exception?.Message ?? "Failed to get files");
        }

        return Ok(response);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(OperationResult<Pagination<FileRecord>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<OperationResult<Pagination<FileRecord>>>> SearchAsync(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? offset = null,
        [FromQuery] int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var request = new SubmissionsSearchRequest(searchTerm, offset, limit);
        var response = await mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Exception?.Message ?? "Failed to search files");
        }

        return Ok(response);
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    [DisableFormValueModelBinding]
    [DisableRequestSizeLimit]
    [ProducesResponseType(typeof(OperationResult<Dictionary<string, Guid>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OperationResult<Dictionary<string, Guid>>>> PutSubmissionAsync(
        [FromServices] FileWithPropertiesFormReader<FileUploadData> formReader,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fileUploadData = formReader.ReadAsync(Request.Body, Request.ContentType ?? string.Empty, cancellationToken);

            var request = new SubmissionsPutRequest(fileUploadData);
            var response = await mediator.Send(request, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Exception?.Message ?? "Failed to upload files");
            }

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("pre-sign")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPreSignAsync(
        [FromQuery] string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token is required");
        }

        var request = new SubmissionGetByPreSignRequest(token);
        var response = await mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
        {
            if (response.Exception is FileNotFoundException)
            {
                return NotFound(response.Exception.Message);
            }

            return BadRequest(response.Exception?.Message ?? "Failed to get file");
        }

        if (response.Result?.Stream == null)
        {
            return NotFound("File stream not found");
        }

        return File(response.Result.Stream, response.Result.FileData.Type, response.Result.FileData.Name);
    }
}