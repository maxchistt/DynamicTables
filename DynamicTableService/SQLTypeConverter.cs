namespace DynamicTableService
{
    public static class SQLTypeConverter
    {
        private static readonly string[] SqlServerTypes = { "bigint", "binary", "bit", "char", "date", "datetime", "datetime2", "datetimeoffset", "decimal", "filestream", "float", "geography", "geometry", "hierarchyid", "image", "int", "money", "nchar", "ntext", "numeric", "nvarchar", "real", "rowversion", "smalldatetime", "smallint", "smallmoney", "sql_variant", "text", "time", "timestamp", "tinyint", "uniqueidentifier", "varbinary", "varchar", "xml" };
        private static readonly string[] CSharpTypes = { "long", "byte[]", "bool", "char", "DateTime", "DateTime", "DateTime", "DateTimeOffset", "decimal", "byte[]", "double", "Microsoft.SqlServer.Types.SqlGeography", "Microsoft.SqlServer.Types.SqlGeometry", "Microsoft.SqlServer.Types.SqlHierarchyId", "byte[]", "int", "decimal", "string", "string", "decimal", "string", "Single", "byte[]", "DateTime", "short", "decimal", "object", "string", "TimeSpan", "byte[]", "byte", "Guid", "byte[]", "string", "string" };

        private static readonly Dictionary<string, Type> TypeDictionary = new() {
            { "long", typeof(long) }, { "byte[]", typeof(byte[]) }, { "bool", typeof(bool) }, { "char", typeof(char) }, { "DateTime", typeof(DateTime) }, { "DateTimeOffset", typeof(DateTimeOffset) }, { "decimal", typeof(decimal) }, { "double", typeof(double) }, { "int", typeof(int) }, { "string", typeof(string) }, { "Single", typeof(Single) }, { "short", typeof(short) }, { "object", typeof(object) }, { "TimeSpan", typeof(TimeSpan) }, { "byte", typeof(byte) }, { "Guid", typeof(Guid) }
        };

        public static List<string> getAvaliableSqlTypeNames()
        {
            var typenamesList = SqlServerTypes.ToList();
            typenamesList = typenamesList.Distinct().ToList();
            return typenamesList;
        }

        public static string ConvertSqlToCS(string typeName)
        {
            var index = Array.IndexOf(SqlServerTypes, typeName);

            return index > -1
                ? CSharpTypes[index]
                : "object";
        }

        public static Type? ConvertSqlToCSType(string typeName)
        {
            return GetTypeFromCSTypeName(ConvertSqlToCS(typeName));
        }

        public static Type? GetTypeFromCSTypeName(string typeName)
        {
            var index = Array.IndexOf(CSharpTypes, typeName);
            if (index > -1 && TypeDictionary.ContainsKey(typeName)) return TypeDictionary[typeName];
            return null;
        }

        public static string ConvertCSToSql(string typeName)
        {
            var index = Array.IndexOf(CSharpTypes, typeName);

            return index > -1
                ? SqlServerTypes[index]
                : null;
        }
    }
}