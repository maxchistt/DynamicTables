namespace DynamicTableService.Components
{
    public class DTScaner : AbstractSqlService
    {
        public DTScaner(string connectionString) : base(connectionString)
        {
        }

        public List<string> getTablesNames()
        {
            string dbName = "";
            var connectionParams = connectionString.Split(';');
            foreach (var param in connectionParams)
            {
                string[] keyAndVal = param.Split('=');
                if (keyAndVal[0].Trim() == "AttachDbFilename")
                {
                    dbName = keyAndVal[1].Trim();
                    break;
                }
            }

            string query = $@"SELECT TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE' {(dbName != "" ? $"AND TABLE_CATALOG='{dbName}'" : "")}";
            var tables = new List<string>();

            var rowsOfTablesData = executeSQLReadArrays(query);

            foreach (var row in rowsOfTablesData)
            {
                string tabName = Convert.ToString(row[0]) ?? "";
                tables.Add(tabName);
            }

            return tables;
        }

        public List<string> getColsKeys(string tableName)
        {
            var cols = getColsKeysTypesAndLen(tableName);
            return cols.Keys.ToList();
        }

        public Dictionary<string, string> getColsKeysAndTypes(string tableName)
        {
            Dictionary<string, string> keyTypenamePairs = new();
            foreach (var col in getColsKeysTypesAndLen(tableName))
            {
                var key = col.Key;
                var type = col.Value.Item1;
                keyTypenamePairs.Add(key, type);
            }
            return keyTypenamePairs;
        }

        public Dictionary<string, Type> getColsKeysAndCSTypes(string tableName)
        {
            string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
            var columnKeysAndTypes = new Dictionary<string, Type>();
            var rowsOfColumnsData = executeSQLReadArrays(query);

            foreach (var row in rowsOfColumnsData)
            {
                string colName = Convert.ToString(row[0]) ?? "";
                Type colType = SQLTypeConverter.ConvertSqlToCSType(Convert.ToString(row[1]) ?? "") ?? typeof(object);
                columnKeysAndTypes.Add(colName, colType);
            }

            return columnKeysAndTypes;
        }

        public Dictionary<string, (string, int?)> getColsKeysTypesAndLen(string tableName)
        {
            string query = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
            var columnKeysTypesAndLen = new Dictionary<string, (string, int?)>();
            var rowsOfColumnsData = executeSQLReadArrays(query);

            foreach (var row in rowsOfColumnsData)
            {
                string colName = Convert.ToString(row[0]) ?? "";
                string colType = Convert.ToString(row[1]) ?? "";
                int? colLen = Convert.ToInt32(row[2]);

                columnKeysTypesAndLen.Add(colName, (colType, colLen));
            }

            return columnKeysTypesAndLen;
        }
    }
}