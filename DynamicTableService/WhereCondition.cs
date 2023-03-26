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
        private object _value;
        private ConditionOperator _operator = ConditionOperator.Equal;

        public WhereCondition(string column, ConditionOperator op, object value)
        {
            _column = column;
            _value = value;
            _operator = op;
        }

        public WhereCondition(string column, string conditionOperator, object value)
        {
            _column = column;
            _value = value;
            _operator = ConditionOperatorsStrings.FirstOrDefault(v => v.Value == conditionOperator).Key;
        }

        private string OperatorToString()
        {
            return ConditionOperatorsStrings[_operator];
        }

        private List<object>? ValueToObjectList()
        {
            var list = new List<object>();

            if (_value is IEnumerable enumerable)
            {
                foreach (var val in enumerable)
                {
                    list.Add(val);
                }
            }
            else if (_value is System.Runtime.CompilerServices.ITuple tuple)
            {
                for (int i = 0; i < tuple.Length; i++)
                {
                    list.Add(tuple[i]);
                }
            }
            else
            {
                return null;
            }

            return list;
        }

        private string ValueToString()
        {
            string resStr = string.Empty;
            switch (_operator)
            {
                case ConditionOperator.In:
                    var list = ValueToObjectList();
                    if (list == null) throw new ArgumentException("Value should be enumerable or tuple if condition needs few values", "object value");
                    resStr = $"({string.Join(", ", list.Select(Components.QueryBuilder.GetValueString))})";
                    break;
                default:
                    resStr = Components.QueryBuilder.GetValueString(_value);
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