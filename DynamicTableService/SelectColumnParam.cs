namespace DynamicTableService
{
    public enum SelectFunction
    { Count, Avg, Sum, Min, Max }

    public struct SelectColumnParam
    {
        string? columnName = null;
        SelectFunction? selectFunction = null;

        public SelectColumnParam(string? columnName, SelectFunction selectFunction)
        {
            this.columnName = columnName;
            this.selectFunction = selectFunction;
        }

        public override string ToString()
        {
            if (selectFunction != null && selectFunction != SelectFunction.Count && (columnName == null || columnName == "*"))
            {
                throw new ArgumentException($"It needs to be a selected field for {getSelFunctionName()}()");
                return string.Empty;
            }
            else
            {
                return selectFunction != null
                    ? $" {getSelFunctionName()}({columnName ?? "*"}) "//+$"AS {colname} "
                    : $" {columnName ?? "*"} ";
            }
        }

        private string getSelFunctionName()
        {
            switch (selectFunction)
            {
                case SelectFunction.Count:
                    return "COUNT";

                case SelectFunction.Sum:
                    return "SUM";

                case SelectFunction.Avg:
                    return "AVG";

                case SelectFunction.Min:
                    return "MIN";

                case SelectFunction.Max:
                    return "MAX";

                default:
                    return string.Empty;
            }
        }

    }
}