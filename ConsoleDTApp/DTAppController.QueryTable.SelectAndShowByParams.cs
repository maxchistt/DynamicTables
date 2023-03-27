using DynamicTableService;

namespace ConsoleDTApp
{
    internal partial class DTAppController
    {
        public void selectAndShowByParams()
        {
            string tableName = chosenTable;

            ///////// columnNames

            List<string>? columnNames = null;
            string chosenField = "*";
            while (chosenField != "")
            {
                chosenField = view.getChoice(dtManager.Scaner.getColsKeys(chosenTable).ToArray(), "Chose field to select. Press enter to end/skip selection.");
                if (chosenField != "")
                {
                    if (columnNames == null) columnNames = new();
                    columnNames.Add(chosenField);
                    view.printMsg(string.Join(" & ", columnNames));

                    if (view.getChoice(new string[] { "Yes", "No" }, "Add another field to select query?") != "Yes")
                    {
                        chosenField = "";
                        break;
                    }
                }
            }

            ///////// whereConditions

            List<WhereCondition>? whereConditions = null;
            bool breakWhereConditions = false;
            while (!breakWhereConditions)
            {
                if (view.getChoice(new string[] { "Yes", "No" }, "Add another where condition?") != "Yes")
                {
                    breakWhereConditions = true;
                    break;
                }

                string column = "";
                while (column == "")
                {
                    column = view.getChoice(dtManager.Scaner.getColsKeys(chosenTable).ToArray(), "Chose field for where condition");
                    if (column == "")
                    {
                        breakWhereConditions = true;
                        break;
                    }
                }
                if (breakWhereConditions) break;

                ///

                ConditionOperator conditionOperator = ConditionOperator.Equal;
                ConditionOperator? op = null;
                while (op == null)
                {
                    op = view.getChoiceEnum<ConditionOperator>("Chose operator for where condition");
                    if (op == null)
                    {
                        breakWhereConditions = true;
                        break;
                    }
                    conditionOperator = op ?? ConditionOperator.Equal;
                }
                if (breakWhereConditions) break;

                ///

                List<object> valuesList = new();
                bool repeatFieldsChoose = true;
                while (repeatFieldsChoose)
                {
                    bool enterStr = (view.getChoice(new string[] { "Number", "String" }, "What type whould you enter?") != "Number");

                    string? valueStr = view.getStringname("Enter value")?.Trim();
                    if (valueStr == null || valueStr == "")
                    {
                        repeatFieldsChoose = false;
                        breakWhereConditions = true;
                        break;
                    }
                    else
                    {
                        if (enterStr)
                        {
                            valuesList.Add(valueStr);
                        }
                        else
                        {
                            double valueDoubleParsed;
                            if (double.TryParse(valueStr, out valueDoubleParsed))
                            {
                                valuesList.Add(valueDoubleParsed);
                            }
                            else
                            {
                                view.printMsg("numeric value wasnt parsed");
                            }
                        }

                        view.printMsg(string.Join(" & ", valuesList));
                    }

                    if (view.getChoice(new string[] { "Yes", "No" }, "Add another value for this condition?") != "Yes") repeatFieldsChoose = false;
                }
                if (breakWhereConditions) break;

                ///

                WhereCondition cond = new WhereCondition(column, conditionOperator, valuesList);

                if (whereConditions == null) whereConditions = new();
                whereConditions.Add(cond);
            }

            ///////// groupByColumnName

            string? groupByColumnName = null;

            ///////// orderByColumnName orderByDescend

            string? orderByColumnName = null;
            bool? orderByDescend = null;

            ///////// offsetAndFetch

            Tuple<int, int>? offsetAndFetch = null;

            /////////

            var table = dtManager.Querier.Select(tableName, columnNames, whereConditions, groupByColumnName, orderByColumnName, orderByDescend, offsetAndFetch);

            string tableStr = $"Table '{chosenTable}'\n";
            table.ForEach(row =>
            {
                tableStr += string.Join(" | ", row) + "\n";
            });
            view.printMsg(tableStr);
        }
    }
}