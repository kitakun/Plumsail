using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Handlers.SubmissionHandlers;
using System.Globalization;
using System.Text.Json;

namespace Plumsail.Interview.Handlers.FormReader;

/// <summary>
/// <see cref="FileUploadData"/> FormField parser implementation
/// </summary>
public sealed class FileUploadFormReader : FileWithPropertiesFormReader<FileUploadData>
{
    protected override void SetProperty(ref FileUploadData form, string propertyName, string value, int? arrayIndex)
    {
        switch (propertyName)
        {
            case "File":
                break;
            default:
                var payload = form.Payload ?? new Dictionary<string, JsonElement>();
                
                if (arrayIndex.HasValue)
                {
                    // Handle array properties like Tags[0], Tags[1], etc.
                    if (!payload.TryGetValue(propertyName, out var existingValue) || existingValue.ValueKind != JsonValueKind.Array)
                    {
                        var list = new List<JsonElement>();
                        payload[propertyName] = JsonSerializer.SerializeToElement(
                            list,
                            HandlersJsonSerializerContext.Default.ListJsonElement);
                    }
                    
                    // Get existing array or create new one
                    var array = payload[propertyName].ValueKind == JsonValueKind.Array 
                        ? payload[propertyName].EnumerateArray().ToList() 
                        : new List<JsonElement>();
                    
                    // Ensure list is large enough
                    while (array.Count <= arrayIndex.Value)
                    {
                        array.Add(JsonSerializer.SerializeToElement(
                            (string?)null,
                            HandlersJsonSerializerContext.Default.String));
                    }
                    
                    // Update the value at the specified index
                    array[arrayIndex.Value] = JsonSerializer.SerializeToElement(
                        value,
                        HandlersJsonSerializerContext.Default.String);
                    payload[propertyName] = JsonSerializer.SerializeToElement(
                        array,
                        HandlersJsonSerializerContext.Default.ListJsonElement);
                }
                else
                {
                    payload[propertyName] = JsonSerializer.SerializeToElement(
                        value,
                        HandlersJsonSerializerContext.Default.String);
                }
                
                form = form with { Payload = payload };
                break;
        }
    }

    protected override void SetFileData(
        ref FileUploadData record,
        string fileName,
        string contentType,
        long size,
        Stream stream)
    {
        record = record with
        {
            FileName = fileName,
            ContentType = contentType,
            Size = size,
            Stream = stream
        };
    }
}