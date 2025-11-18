using System.Data;
using System.Text.Json;

using Dapper;

namespace Plumsail.Interview.DatabaseContext.TypeHandlers;

public sealed class JsonElementTypeHandler : SqlMapper.TypeHandler<JsonElement>
{
    public override void SetValue(IDbDataParameter parameter, JsonElement value)
    {
        parameter.Value = value.ValueKind != JsonValueKind.Undefined
            ? value.GetRawText()
            : "{}";
    }

    public override JsonElement Parse(object value)
    {
        if (value == null || value == DBNull.Value)
        {
            using var emptyDocument = JsonDocument.Parse("{}");
            return emptyDocument.RootElement.Clone();
        }

        var json = value.ToString() ?? "{}";
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}