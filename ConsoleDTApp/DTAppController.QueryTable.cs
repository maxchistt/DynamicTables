using DynamicTableService;
using System.ComponentModel;

namespace ConsoleDTApp
{
    internal partial class DTAppController
    {
        private enum todoQueryTable
        { ShowTable, AddRow, SelectWithParams };

        public void queryTable()
        {
            bool tableCosen = chooseTable();
            if (!tableCosen) return;

            var keysAndTypes = dtManager.Scaner.getColsKeysAndTypes(chosenTable);
            view.printMsg($"'{chosenTable}' table keys and types:\n{string.Join(" | ", keysAndTypes)}");

            bool resume = true;
            while (resume)
            {
                resume = chooseToDoQueryTable();
            }
        }

        public bool chooseToDoQueryTable()
        {
            var todoQueryTableRes = view.getChoiceEnum<todoQueryTable>("Choose what to do");
            if (todoQueryTableRes == null) return false;
            switch (todoQueryTableRes)
            {
                case todoQueryTable.ShowTable:
                    showTable();
                    break;

                case todoQueryTable.AddRow:
                    view.printMsg(addRow() ? "Row added" : "Row not added");
                    break;

                case todoQueryTable.SelectWithParams:
                    selectAndShow();
                    break;

                default:
                    view.printMsg("Nothing chosen to do");
                    return false;
            }
            return true;
        }

        public void selectAndShow()
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

        public void showTable()
        {
            var table = dtManager.Querier.Select(chosenTable);

            string tableStr = $"Table '{chosenTable}'\n";
            table.ForEach(row =>
            {
                tableStr += string.Join(" | ", row) + "\n";
            });
            view.printMsg(tableStr);
        }

        public bool addRow()
        {
            Dictionary<string, object> row = new();
            var colsKeysAndTypes = dtManager.Scaner.getColsKeysAndCSTypes(chosenTable);
            foreach (var col in colsKeysAndTypes)
            {
            // label
            repeat:
                var type = col.Value;
                var input = view.getStringname($"Enter {type.Name} value for {col.Key} column:");
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null && input != null && input != "")
                {
                    var convres = converter.ConvertFromString(input);
                    if (convres != null)
                    {
                        row[col.Key] = convres;
                        continue;
                    }
                }
                var choiceRes = view.getChoice(new[] { "No value", "Null", "Empty str", "Rechoose" }, $"No value. Chose what to set as {col.Key}:");
                switch (choiceRes)
                {
                    case "No value":
                        continue;
                    case "Null":
                        row[col.Key] = null;
                        break;

                    case "Empty str":
                        row[col.Key] = "";
                        break;

                    case "Rechoose":
                        goto repeat;
                }
            }

            view.printMsg("Row:\n" + string.Join(" | ", row));
            if (view.getChoice(new[] { "Insert row", "Dont insert row" }) == "Insert row")
            {
                dtManager.Querier.Insert(chosenTable, row);
                return true;
            }
            return false;
        }
    }
}