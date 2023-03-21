namespace DynamicTableService.Components
{
    public class DTQuerier : AbstractSqlService
    {
        protected QueryBuilder queryBuilder;

        public DTQuerier(string connectionString) : base(connectionString)
        {
            queryBuilder = new QueryBuilder(connectionString);
        }

        public DTQuerier(string connectionString, string tableName) : this(connectionString)
        {
            setTable(tableName);
        }

        public void setTable(string tableName)
        {
            queryBuilder.Table(tableName);
        }

        public void Insert(Dictionary<string, object> values)
        {
            queryBuilder.ResetOptions().Insert(values);
            executeSQL(queryBuilder.Build());
        }

        public void Update(Dictionary<string, object> values, List<WhereCondition>? whereConditions = null)
        {
            queryBuilder.ResetOptions().Update(values);
            if (whereConditions != null) whereConditions.ForEach(condition => queryBuilder.Where(condition));
            executeSQL(queryBuilder.Build());
        }

        public void Delete(List<WhereCondition> whereConditions)
        {
            queryBuilder.ResetOptions().Delete();
            whereConditions.ForEach(condition => queryBuilder.Where(condition));
            executeSQL(queryBuilder.Build());
        }

        public List<Dictionary<string, object>> Select(List<string>? columnNames = null, List<WhereCondition>? whereConditions = null, string? orderByColumnName = null)
        {
            queryBuilder.ResetOptions().Select(columnNames?.ToArray() ?? new string[0]);
            if (whereConditions != null) whereConditions.ForEach(condition => queryBuilder.Where(condition));
            if (orderByColumnName != null) queryBuilder.OrderBy(orderByColumnName);
            string sql = queryBuilder.Build();

            return executeSQLReadDictionaries(sql);
        }
    }
}