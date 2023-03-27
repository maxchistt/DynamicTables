﻿using DynamicTableService;

namespace ConsoleDTApp
{
    internal partial class DTAppController
    {
        public void selectAndShowByParams()
        {
            string tableName = chosenTable;

            ///////// columnNames

            List<string>? columnNames = null;
            while (true)
            {
                if (view.getChoice(new string[] { "Yes", "No" }, "Add another field to SELECT query?") != "Yes")
                {
                    break;
                }

                string chosenField = view.getChoice(dtManager.Scaner.getColsKeys(chosenTable).ToArray(), "Choose field to SELECT. Press enter to end/skip selection.");
                if (chosenField != "")
                {
                    if (columnNames == null) columnNames = new();
                    columnNames.Add(chosenField);
                    view.printMsg("Selected columns: " + string.Join(" | ", columnNames));
                }
            }

            ///////// whereConditions

            List<WhereCondition>? whereConditions = null;
            bool breakWhereConditions = false;
            while (!breakWhereConditions)
            {
                if (view.getChoice(new string[] { "Yes", "No" }, "Add another WHERE condition?") != "Yes")
                {
                    breakWhereConditions = true;
                    break;
                }

                string column = "";
                while (column == "")
                {
                    column = view.getChoice(dtManager.Scaner.getColsKeys(chosenTable).ToArray(), "Choose field for WHERE condition");
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
                    op = view.getChoiceEnum<ConditionOperator>("Choose operator for WHERE condition");
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
                    string enterType = view.getChoice(new string[] { "Number", "String", "Sql string" }, "What type whould you enter?");

                    string? valueStr = view.getStringname("Enter value:")?.Trim();
                    if (valueStr == null || valueStr == "")
                    {
                        repeatFieldsChoose = false;
                        breakWhereConditions = true;
                        break;
                    }
                    else
                    {
                        switch (enterType)
                        {
                            case "Number":
                                double valueDoubleParsed;
                                if (double.TryParse(valueStr, out valueDoubleParsed))
                                {
                                    valuesList.Add(valueDoubleParsed);
                                }
                                else
                                {
                                    view.printMsg("numeric value wasnt parsed");
                                }
                                break;

                            case "Sql string":
                                valuesList.Add(valueStr);
                                break;

                            default:
                                valuesList.Add($"'{valueStr}'");
                                break;
                        }

                        view.printMsg("Values: " + string.Join(" , ", valuesList));
                    }

                    if (view.getChoice(new string[] { "Yes", "No" }, "Add another value for this condition?") != "Yes") repeatFieldsChoose = false;
                }
                if (breakWhereConditions) break;

                ///

                WhereCondition cond = new WhereCondition(column, conditionOperator, valuesList, false);

                if (whereConditions == null) whereConditions = new();
                whereConditions.Add(cond);

                view.printMsg("Conditions: " + string.Join(" AND ", whereConditions.Select(cond => cond.ToString())));
            }

            ///////// groupByColumnName

            string? groupByColumnName = null;

            while (groupByColumnName == null)
            {
                if (view.getChoice(new string[] { "Yes", "No" }, "Add GROUP BY query?") != "Yes")
                {
                    groupByColumnName = null;
                    break;
                }

                groupByColumnName = view.getChoice(dtManager.Scaner.getColsKeys(chosenTable).ToArray(), "Choose field to SELECT. Press enter to end/skip selection.");
                if (groupByColumnName == "")
                {
                    groupByColumnName = null;
                }
            }

            ///////// orderByColumnName orderByDescend

            string? orderByColumnName = null;
            bool? orderByDescend = null;

            ///////// offsetAndFetch

            Tuple<int, int>? offsetAndFetch = null;

            /////////

            var table = dtManager.Querier.Select(tableName, columnNames, whereConditions, groupByColumnName, orderByColumnName, orderByDescend, offsetAndFetch);

            printTable(table);
        }
    }
}