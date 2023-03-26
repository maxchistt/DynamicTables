using System.Collections;
namespace DynamicTableService
{
    public enum ConditionOperator
    { Equal, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, NotEqual, In }

    public struct WhereCondition
    {
        private static Dictionary<ConditionOperator, string> ConditionOperatorsStrings = new(){
            { ConditionOperator.Equal, "="},
            { ConditionOperator.GreaterThan, ">"},
            { ConditionOperator.LessThan, "<"},
            { ConditionOperator.GreaterThanOrEqual, ">="},
            { ConditionOperator.LessThanOrEqual, "<="},
            { ConditionOperator.NotEqual, "!="},
            { ConditionOperator.In, "IN"},
        };

        private string _column = "col1";
        private object[] _values = new object[0];
        private ConditionOperator _operator = ConditionOperator.Equal;

        public WhereCondition(string column, ConditionOperator op, params object[] values)
        {
            _column = column;

            if (values.Length > 1)
            {
                _values = values;
            }
            else if (values.Length == 1)
            {
                _values = this.GetObjectListFromObject(values[0]);
            }
            else
            {
                throw new Exception("No values in WHERE condition");
            }

            _operator = op;
        }

        public WhereCondition(string column, string conditionOperator, params object[] values) : this(column, ConditionOperatorsStrings.FirstOrDefault(v => v.Value == conditionOperator).Key, values) { }

        private string OperatorToString()
        {
            return ConditionOperatorsStrings[_operator];
        }

        private object[] GetObjectListFromObject(object objVal)
        {
            var list = new List<object>();

            if (objVal is IEnumerable enumerable)
            {
                foreach (var val in enumerable)
                {
                    list.Add(val);
                }
            }
            else if (objVal is System.Runtime.CompilerServices.ITuple tuple)
            {
                for (int i = 0; i < tuple.Length; i++)
                {
                    list.Add(tuple[i]);
                }
            }
            else
            {
                list.Add(objVal);
            }

            return list.ToArray();
        }

        private string ValueToString()
        {
            string resStr = string.Empty;
            switch (_operator)
            {
                case ConditionOperator.In:

                    if (_values.Length <= 1) throw new ArgumentException("Value should be enumerable or tuple if condition needs few values", "object value");
                    resStr = $"({string.Join(", ", _values.Select(Components.QueryBuilder.GetValueString))})";
                    break;
                default:
                    resStr = Components.QueryBuilder.GetValueString(_values[0]);
                    break;
            }
            return resStr;
        }

        public new string ToString()
        {
            return $"{_column} {OperatorToString()} {ValueToString()}";
        }
    }
}