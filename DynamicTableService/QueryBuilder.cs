using System.Text.RegularExpressions;

namespace DynamicTableService.Components
{
    public class QueryBuilder
    {
        private string _tableName;
        private List<string> _selectColumns;
        private List<string> _whereConditions;
        private Dictionary<string, object> _values;
        private string? _orderByColumn;
        private bool _orderByDesc = false;
        private string? _groupByColumn;
        private Tuple<int, int>? _offsetFetch;

        private enum QueryType
        { Select, Insert, Update, Delete }

        private QueryType _queryType;

        public QueryBuilder ResetOptions()
        {
            _selectColumns = new List<string>();
            _whereConditions = new List<string>();
            _values = new Dictionary<string, object>();
            _orderByColumn = null;
            _orderByDesc = false;
            _groupByColumn = null;
            _offsetFetch = null;
            _queryType = QueryType.Select;
            return this;
        }

        public QueryBuilder() : this("MyTable")
        {
        }

        public QueryBuilder(string tableName)
        {
            ResetOptions();
            Table(tableName);

            /*
            _tableName = tableName;
            _selectColumns = new List<string>();
            _whereConditions = new List<string>();
            _values = new Dictionary<string, object>();
            _orderByColumn = null;
            _queryType = QueryType.Select;
            */
        }

        public QueryBuilder Table(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public QueryBuilder Select(params string[] columns)
        {
            _queryType = QueryType.Select;
            _selectColumns.AddRange(columns);
            return this;
        }

        public QueryBuilder Where(string sql_full_condition)
        {
            _whereConditions.Add(sql_full_condition);
            return this;
        }

        public QueryBuilder Where(WhereCondition condition)
        {
            return Where(condition.ToString());
        }

        /*
        public QueryBuilder Where(string column, ConditionOperator conditionOperator, object conditionValue)
        {
            return Where(new WhereCondition(column, conditionOperator, conditionValue).ToString());
        }

        public QueryBuilder Where(string column, string conditionOperator, object conditionValue)
        {
            return Where(new WhereCondition(column, conditionOperator, conditionValue).ToString());
        }
        */

        public QueryBuilder GroupBy(string column)
        {
            _groupByColumn = column;
            return this;
        }

        public QueryBuilder OrderBy(string column, bool byDescend = false)
        {
            _orderByColumn = column;
            _orderByDesc = byDescend;
            return this;
        }

        public QueryBuilder offsetFetch(int offset, int fetch)
        {
            _offsetFetch = new(offset, fetch);
            return this;
        }


        public QueryBuilder Update(Dictionary<string, object> values)
        {
            _queryType = QueryType.Update;
            _values = values;
            return this;
        }

        public QueryBuilder Insert(Dictionary<string, object> values)
        {
            _queryType = QueryType.Insert;
            _values = values;
            return this;
        }

        public QueryBuilder Delete()
        {
            _queryType = QueryType.Delete;
            return this;
        }

        private string BuildSelect()
        {
            string selectClause = _selectColumns.Count > 0 ? string.Join(", ", _selectColumns) : "*";
            string whereClause = _whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", _whereConditions) : "";
            string groupByClause = !string.IsNullOrEmpty(_groupByColumn) ? $"GROUP BY {_groupByColumn}" : "";
            string orderByClause = !string.IsNullOrEmpty(_orderByColumn) ? $"ORDER BY {_orderByColumn} {(_orderByDesc ? "DESC" : "")}" : "";
            string offsetFetchClause = _offsetFetch != null ? $"OFFSET {_offsetFetch.Item1} ROWS FETCH NEXT {_offsetFetch.Item2} ROWS ONLY" : "";
            return $"SELECT {selectClause} FROM {_tableName} {whereClause} {groupByClause} {orderByClause} {offsetFetchClause}";
        }

        private string BuildUpdate()
        {
            string setClause = string.Join(", ", _values.Select(pair => $"{pair.Key} = {GetValueString(pair.Value)}"));
            string whereClause = _whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", _whereConditions) : "";
            return $"UPDATE {_tableName} SET {setClause} {whereClause}";
        }

        private string BuildInsert()
        {
            string columns = string.Join(", ", _values.Keys);
            string values = string.Join(", ", _values.Values.Select(GetValueString));
            return $@"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
        }

        private string BuildDelete()
        {
            string whereClause = _whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", _whereConditions) : "";
            return $"DELETE FROM {_tableName} {whereClause}";
        }

        public static string GetValueString(object value)
        {
            return GetValueString(value, true);
        }
        public static string GetValueString(object value, bool autoQuote = true)
        {
            if (value is string)
            {
                return Regex.IsMatch(value.ToString(), @"\p{IsCyrillic}")
                    ? (autoQuote ? $"N'{value}'" : $"{value}")
                    : (autoQuote ? $"'{value}'" : $"{value}");
            }
            else if (value is null)
            {
                return "NULL";
            }
            else
            {
                return value.ToString().Replace(',', '.');
            }
        }

        public string Build(bool resetParams = true)
        {
            string sql;
            switch (_queryType)
            {
                case QueryType.Select:
                    sql = BuildSelect();
                    break;

                case QueryType.Insert:
                    sql = BuildInsert();
                    break;

                case QueryType.Update:
                    sql = BuildUpdate();
                    break;

                case QueryType.Delete:
                    sql = BuildDelete();
                    break;

                default:
                    sql = string.Empty;
                    //throw new NotImplementedException();
                    break;
            }

            if (resetParams) ResetOptions();

            return sql;
        }
    }
}