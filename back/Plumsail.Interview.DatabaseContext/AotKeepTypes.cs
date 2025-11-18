namespace Plumsail.Interview.DatabaseContext;

public static class AotKeepTypes
{
    public static void EnsureTypesArePreserved()
    {
        _ = typeof(Dapper.SqlMapper);
        _ = typeof(Dapper.CommandDefinition);
        _ = typeof(System.Data.Common.DbConnection);
        _ = typeof(System.Data.Common.DbTransaction);
    }
}