namespace DynamicTableService.Components
{
    public enum ConditionOperator
    { Equal, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, NotEqual }

    public struct WhereCondition
    {
        private static Dictionary<ConditionOperator, string> ConditionOperatorsStrings = new(){
            { ConditionOperator.Equal, "="},
            { ConditionOperator.GreaterThan, ">"},
            { ConditionOperator.LessThan, "<"},
            { ConditionOperator.GreaterThanOrEqual, ">="},
            { ConditionOperator.LessThanOrEqual, "<="},
            { ConditionOperator.NotEqual, "!="},
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

        private static string OperatorToString(ConditionOperator _operator)
        {
            return ConditionOperatorsStrings[_operator];
        }

        public new string ToString()
        {
            return $"{_column} {OperatorToString(_operator)} {Components.QueryBuilder.GetValueString(_value)}";
        }
    }
}