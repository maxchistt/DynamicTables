using System.Text.RegularExpressions;

namespace DynamicTableService
{
    public struct WhereCondition
    {
        public enum ConditionOperator
        { Equal, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, NotEqual }

        private string _column = "col1";
        private object _value;
        private ConditionOperator _operator = ConditionOperator.Equal;

        public WhereCondition(string column, object value, ConditionOperator op = ConditionOperator.Equal)
        {
            _column = column;
            _value = value;
            _operator = op;
        }

        private static string OperatorToString(ConditionOperator _operator)
        {
            switch (_operator)
            {
                case ConditionOperator.Equal: return "=";
                case ConditionOperator.GreaterThan: return ">";
                case ConditionOperator.LessThan: return "<";
                case ConditionOperator.GreaterThanOrEqual: return ">=";
                case ConditionOperator.LessThanOrEqual: return "<=";
                case ConditionOperator.NotEqual: return "!=";
                default: return "=";
            }
        }

        public new string ToString()
        {
            return $"{_column} {OperatorToString(_operator)} {Components.QueryBuilder.GetValueString(_value)}";
        }
    }
}

namespace DynamicTableService.Components
{
    public class QueryBuilder
    {
        private string _tableName;
        private List<string> _selectColumns;
        private List<string> _whereConditions;
        private Dictionary<string, object> _values;
        private string? _orderByColumn;

        private enum QueryType
        { Select, Insert, Update, Delete }

        private QueryType _queryType;

        public QueryBuilder ResetOptions()
        {
            _selectColumns = new List<string>();
            _whereConditions = new List<string>();
            _values = new Dictionary<string, object>();
            _orderByColumn = null;
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

        private QueryBuilder Where(string condition)
        {
            _whereConditions.Add(condition);
            return this;
        }

        public QueryBuilder Where(WhereCondition condition)
        {
            return Where(condition.ToString());
        }

        public QueryBuilder Where(string column, WhereCondition.ConditionOperator conditionOperator, object conditionValue)
        {
            return Where(new WhereCondition(column, conditionValue, conditionOperator).ToString());
        }

        public QueryBuilder OrderBy(string column)
        {
            _orderByColumn = column;
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
            string orderByClause = !string.IsNullOrEmpty(_orderByColumn) ? $"ORDER BY {_orderByColumn}" : "";
            return $"SELECT {selectClause} FROM {_tableName} {whereClause} {orderByClause}";
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
            if (value is string)
            {
                return Regex.IsMatch(value.ToString(), @"\p{IsCyrillic}") ? $"N'{value}'" : $"'{value}'";
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