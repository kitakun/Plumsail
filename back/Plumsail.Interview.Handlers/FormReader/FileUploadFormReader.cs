using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Handlers.SubmissionHandlers;

using System.Globalization;

namespace Plumsail.Interview.Handlers.FormReader;

/// <summary>
/// <see cref="FileUploadData"/> FormField parser implementation
/// </summary>
public sealed class FileUploadFormReader : FileWithPropertiesFormReader<FileUploadData>
{
    protected override void SetProperty(ref FileUploadData form, string propertyName, string value)
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

            default:
                throw new NotImplementedException($"Unknown property {propertyName}");
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

    protected override bool HasValidFileData(FileUploadData record)
    {
        return !string.IsNullOrEmpty(record.FileName) && record.Stream != null;
    }
}