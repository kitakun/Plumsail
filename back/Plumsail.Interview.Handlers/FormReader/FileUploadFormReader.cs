using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Handlers.SubmissionHandlers;

using System.Globalization;

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
            case nameof(FileUploadData.Description):
                form = form with { Description = value };
                break;
            case nameof(FileUploadData.Status):
                if (Enum.TryParse<SubmissionStatusEnum>(value, true, out var status))
                {
                    form = form with { Status = status };
                }

                break;
            case nameof(FileUploadData.Priority):
                if (Enum.TryParse<PriorityLevelEnum>(value, true, out var priority))
                {
                    form = form with { Priority = priority };
                }

                break;
            case nameof(FileUploadData.IsPublic):
                if (bool.TryParse(value, out var isPublic))
                {
                    form = form with { IsPublic = isPublic };
                }

                break;
            case nameof(FileUploadData.CreatedDate):
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var createdDate))
                {
                    form = form with { CreatedDate = createdDate };
                }

                break;
            case "File":
                break;
            default:
                var payload = form.Payload ?? new Dictionary<string, object>();
                
                if (arrayIndex.HasValue)
                {
                    // Handle array properties like Tags[0], Tags[1], etc.
                    if (!payload.TryGetValue(propertyName, out var existingValue) || existingValue is not List<object> list)
                    {
                        list = new List<object>();
                        payload[propertyName] = list;
                    }
                    
                    // Ensure list is large enough
                    while (list.Count <= arrayIndex.Value)
                    {
                        list.Add(null!);
                    }
                    
                    list[arrayIndex.Value] = value;
                }
                else
                {
                    payload[propertyName] = value;
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