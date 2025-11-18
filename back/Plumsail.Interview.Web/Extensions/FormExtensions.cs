using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Plumsail.Interview.Handlers.SubmissionHandlers;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Plumsail.Interview.Web.Extensions;

public static class FormExtensions
{
    private const long DefaultMaxRequestSizeMegabytes = 10240; // 10GB default
    private const long BytesPerMegabyte = 1024 * 1024;

    public static void ConfigureFormLimits(this IServiceCollection services, IConfiguration configuration)
    {
        var maxRequestSizeMegabytes = configuration.GetValue("FormLimits:MaxRequestSizeMegabytes", DefaultMaxRequestSizeMegabytes);
        var maxRequestSizeBytes = maxRequestSizeMegabytes * BytesPerMegabyte;

        // Configure request size limits and disable buffering for large file uploads
        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue;
            options.BufferBody = false;
            options.BufferBodyLengthLimit = long.MaxValue;
            options.ValueLengthLimit = int.MaxValue;
            options.ValueCountLimit = int.MaxValue;
            options.MemoryBufferThreshold = int.MaxValue;
        });

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = maxRequestSizeBytes;
            // Set a small buffer size (64KB) for reading multipart boundaries and headers
            options.Limits.MaxRequestBufferSize = 64 * 1024; // 64KB buffer for boundaries/headers
            options.AllowSynchronousIO = false;
        });

        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = maxRequestSizeBytes;
            options.AllowSynchronousIO = false;
        });
    }

    public static async IAsyncEnumerable<FileUploadData> CreateSingleJsonObjectAsyncEnumerator(Stream body, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(body, System.Text.Encoding.UTF8, leaveOpen: false);
        var jsonContent = await streamReader.ReadToEndAsync(cancellationToken);
        
        var jsonDocument = JsonDocument.Parse(jsonContent);
        var payload = new Dictionary<string, JsonElement>();
        
        foreach (var property in jsonDocument.RootElement.EnumerateObject())
        {
            payload[property.Name] = property.Value.Clone();
        }

        var jsonFileUploadData = new FileUploadData(
            FileName: string.Empty,
            Size: 0,
            ContentType: "application/json",
            Stream: Stream.Null,
            Payload: payload
        );

        yield return jsonFileUploadData;
    }
}