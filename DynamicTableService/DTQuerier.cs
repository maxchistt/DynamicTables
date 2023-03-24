namespace DynamicTableService.Components
{
    public class DTQuerier : AbstractSqlService
    {
        protected QueryBuilder queryBuilder;

        public DTQuerier(string connectionString) : base(connectionString)
        {
            queryBuilder = new QueryBuilder(connectionString);
        }

        public void Insert(string tableName, Dictionary<string, object> values)
        {
            queryBuilder.ResetOptions().Table(tableName).Insert(values);
            executeSQL(queryBuilder.Build());
        }

        public void Update(string tableName, Dictionary<string, object> values, List<WhereCondition>? whereConditions = null)
        {
            queryBuilder.ResetOptions().Table(tableName).Update(values);
            if (whereConditions != null) whereConditions.ForEach(condition => queryBuilder.Where(condition));
            executeSQL(queryBuilder.Build());
        }

        public void Delete(string tableName, List<WhereCondition> whereConditions)
        {
            queryBuilder.ResetOptions().Table(tableName).Delete();
            whereConditions.ForEach(condition => queryBuilder.Where(condition));
            executeSQL(queryBuilder.Build());
        }

        public List<Dictionary<string, object>> Select(string tableName, List<string>? columnNames = null, List<WhereCondition>? whereConditions = null, string? orderByColumnName = null)
        {
            queryBuilder.ResetOptions().Table(tableName).Select(columnNames?.ToArray() ?? new string[0]);
            if (whereConditions != null) whereConditions.ForEach(condition => queryBuilder.Where(condition));
            if (orderByColumnName != null) queryBuilder.OrderBy(orderByColumnName);
            string sql = queryBuilder.Build();

            return executeSQLReadDictionaries(sql);
        }
    }
}