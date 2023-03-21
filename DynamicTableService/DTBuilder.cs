namespace DynamicTableService.Components
{
    public class DTBuilder : AbstractSqlService
    {
        public DTBuilder(string connectionString) : base(connectionString)
        {
        }

        public List<string> getAllSqlTypesList()
        {
            return SQLTypeConverter.getAvaliableSqlTypeNames();
        }

        public void CreateTable(string tableName)
        {
            string sqlExpression = $@"
            IF (NOT EXISTS (SELECT *
               FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA = 'dbo'
               AND TABLE_NAME = '{tableName}'))
               BEGIN
                    CREATE TABLE [dbo].[{tableName}] ();
               END;
            ";

            executeSQL(sqlExpression);
        }

        public void RenameTable(string tableNameBefore, string tableNameAfter)
        {
            string sqlExpression = $@"EXEC sp_rename '{tableNameBefore}', '{tableNameAfter}'";

            executeSQL(sqlExpression);
        }

        public void AddColumn(string tableName, string columnName, string type = "nvarchar", string? length = "50", bool? nullable = true)
        {
            string sqlExpression = $@"ALTER TABLE {tableName}
                  ADD {columnName} {type}{(length != null ? $"({length})" : "")} {(nullable != null ? (nullable.Value ? "NULL" : "NOT NULL") : "")};";

            executeSQL(sqlExpression);
        }

        public void EditColumn(string tableName, string columnName, string type = "int", string? length = null, bool? nullable = true)
        {
            string sqlExpression = $@"ALTER TABLE {tableName}
                  ALTER {columnName} {type}{(length != null ? $"({length})" : "")} {(nullable != null ? (nullable.Value ? "NULL" : "NOT NULL") : "")};";

            executeSQL(sqlExpression);
        }

        public void RenameColumn(string tableName, string columnNameBefore, string columnNameAfter)
        {
            string sqlExpression = $@"ALTER TABLE {tableName}
                  RENAME COLUMN {columnNameBefore} to {columnNameAfter};";

            executeSQL(sqlExpression);
        }

        public void AddPrimaryKey(string tableName, string primaryKeyColumnName)
        {
            string sqlExpression = $@"ALTER TABLE {tableName}
                  ADD CONSTRAINT PK_{tableName} PRIMARY KEY ({primaryKeyColumnName});";

            executeSQL(sqlExpression);
        }

        public void AddPrimaryKeys(string tableName, List<string> primaryKeyColumnNames)
        {
            string sqlExpression = $@"ALTER TABLE {tableName}
                  ADD CONSTRAINT PK_{tableName} PRIMARY KEY ({string.Join(", ", primaryKeyColumnNames)});";

            executeSQL(sqlExpression);
        }

        public void DropColumn(string tableName, string columnName)
        {
            string sqlExpression = $@"ALTER TABLE {tableName}
                  DROP {columnName};";

            executeSQL(sqlExpression);
        }

        public void DropTable(string tableName)
        {
            string sqlExpression = $@"DROP TABLE {tableName}";

            executeSQL(sqlExpression);
        }

        public void DropPrimaryKeys(string tableName)
        {
            string sqlExpression = $@"ALTER TABLE {tableName}
                  DROP CONSTRAINT PK_{tableName};";

            executeSQL(sqlExpression);
        }
    }
}