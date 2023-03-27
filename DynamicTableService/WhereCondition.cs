﻿using System.Collections;

namespace DynamicTableService
{
    public enum ConditionOperator
    { Equal, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, NotEqual, In, Between, Like }

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
            { ConditionOperator.Between,"BETWEEN"},
            { ConditionOperator.Like,"LIKE"}
        };

        private string _column = "col1";
        private List<object> _values = new();
        private ConditionOperator _operator = ConditionOperator.Equal;
        private bool _autoQuotes = true;

        public WhereCondition(string column, string conditionOperator, IEnumerable<object> valuesList, bool autoQuotes = true) : this(column, ConditionOperatorsStrings.FirstOrDefault(v => v.Value == conditionOperator).Key, valuesList, autoQuotes) { }

        public WhereCondition(string column, ConditionOperator op, IEnumerable<object> valuesList, bool autoQuotes = true)
        {
            _autoQuotes = autoQuotes;
            _column = column;

            if (valuesList.Count() > 0)
            {
                _values.AddRange(valuesList);
            }
            else
            {
                throw new ArgumentException("No values in WHERE condition IEnumerable<object> valuesList");
            }

            _operator = op;
        }

        private string OperatorToString()
        {
            return ConditionOperatorsStrings[_operator];
        }

        private List<object> GetObjectListFromObject(object objVal)
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

            if (list.Count == 0) throw new Exception("No values in WHERE condition object[]");
            return list;
        }

        private string ValueToString()
        {
            string resStr = string.Empty;
            switch (_operator)
            {
                case ConditionOperator.In:
                    if (_values.Count < 2) throw new ArgumentException("WhereCondition needs few values");
                    bool autoQuote = this._autoQuotes;
                    resStr = $"({string.Join(", ", _values.Select(val => Components.QueryBuilder.GetValueString(val, autoQuote)))})";
                    break;

                case ConditionOperator.Between:
                    if (_values.Count < 2) throw new ArgumentException("WhereCondition needs few values");
                    resStr = $"{_values[0]} AND {_values[1]}";
                    break;

                case ConditionOperator.Like:
                    string search = _values[0].ToString() ?? "";
                    var specSimbols = new char[] { '%', '[', ']', '_' };
                    bool containsSpecSimbols = false;
                    foreach (var simbol in specSimbols)
                    {
                        if (search.Contains(simbol))
                        {
                            containsSpecSimbols = true;
                            break;
                        }
                    }
                    resStr = containsSpecSimbols ? $"'{search}'" : $"'%{search}%'";
                    break;

                default:
                    resStr = Components.QueryBuilder.GetValueString(_values[0], _autoQuotes);
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