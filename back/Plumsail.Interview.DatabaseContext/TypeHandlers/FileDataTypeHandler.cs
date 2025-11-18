using System.Data;
using System.Text.Json;

using Dapper;

using Plumsail.Interview.Domain.Entities;

namespace Plumsail.Interview.DatabaseContext.TypeHandlers;

public sealed class FileDataTypeHandler : SqlMapper.TypeHandler<FileData>
{
    public override void SetValue(IDbDataParameter parameter, FileData value)
    {
        parameter.Value = value == default
            ? DBNull.Value
            : JsonSerializer.Serialize(value, DatabaseJsonSerializerContext.Default.FileData);
    }

    public override FileData Parse(object value)
    {
        if (value == default || value == DBNull.Value)
        {
            return default;
        }

        var json = value.ToString();
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<FileData>(json, DatabaseJsonSerializerContext.Default.FileData);
    }
}