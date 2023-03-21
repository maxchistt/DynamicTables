using DynamicTableService;
using System.ComponentModel;

namespace ConsoleDTApp
{
    internal class TableConsole
    {
        private DynamicTableManager dtManager = new();
        private string chosenTable = "";
        private ConsoleView view = new();

        private enum todoMain
        { ChooseTable, CreateTable, EditTable, ShowTablesList, QueryTable };

        private enum todoEditTable
        { RenameTable, AddColumn, EditColumn, RenameColumn, AddPrimaryKey, AddPriamryKeys, DropTable, DropColumn, DropPrimaryKeys };

        private enum todoQueryTable
        { ShowTable, AddRow };

        public TableConsole()
        {
            bool resume = true;
            while (resume)
            {
                resume = chooseToDoMain();
            }
        }

        public bool chooseToDoMain()
        {
            var todoMainRes = view.getChoiceEnum<todoMain>("Choose what to do");
            if (todoMainRes == null) return false;
            switch (todoMainRes)
            {
                case todoMain.CreateTable:
                    view.printMsg(createTable() ? "Table creted" : "No table created");
                    break;

                case todoMain.ChooseTable:
                    view.printMsg(chooseTable()
                        ? $"Chosen table is '{chosenTable}':\n{string.Join(" | ", dtManager.Scaner.getColsKeysAndTypes(chosenTable))}"
                        : "No table chosen");
                    break;

                case todoMain.ShowTablesList:
                    showTablesList();
                    break;

                case todoMain.EditTable:
                    editTable();
                    break;

                case todoMain.QueryTable:
                    queryTable();
                    break;

                default:
                    view.printMsg("Nothing chosen to do");
                    return false;
            }
            return true;
        }

        public void showTablesList()
        {
            var tabnames = dtManager.Scaner.getTablesNames();
            view.printMsg(tabnames.Count > 0
                ? $"Here is the list of tables:\n {string.Join(", ", tabnames)}"
                : "No tables in Db");
        }

        public bool createTable()
        {
            var tabName = view.getStringname("Enter name of table to create:");
            if (tabName == null || tabName == "") return false;
            dtManager.Builder.CreateTable(tabName);
            return true;
        }

        public void editTable()
        {
            bool tableCosen = chooseTable();
            if (!tableCosen) return;

            var keysAndTypes = dtManager.Scaner.getColsKeysAndTypes(chosenTable);
            view.printMsg($"'{chosenTable}' table keys and types:\n{string.Join(" | ", keysAndTypes)}");

            bool resume = true;
            while (resume)
            {
                resume = chooseToDoEditTable();
            }
        }

        public bool chooseToDoEditTable()
        {
            var todoEditTableRes = view.getChoiceEnum<todoEditTable>("Choose what to do");
            if (todoEditTableRes == null) return false;
            switch (todoEditTableRes)
            {
                case todoEditTable.AddColumn:
                    addColumn();
                    break;
                default:
                    view.printMsg("Nothing chosen to do");
                    return false;
            }
            return true;
        }

        public void addColumn()
        {
            string colname;
            string type;
            string? len;
            bool nullable;

            colname = view.getStringname("Write column name:") ?? "";
            if (colname == "") return;

            type = view.getChoice(dtManager.Builder.getAllSqlTypesList().ToArray(), "Choose column type:");
            if (type == "") return;

            len = view.getStringname("Enter length or just press 'Enter':");
            if (len == null || !int.TryParse(len, out _)) len = null;

            nullable = view.getChoice(new[] { "Null", "Not Null" }, "Choose if nullable:") != "Not Null";

            dtManager.Builder.AddColumn(chosenTable, colname, type, len, nullable);
        }

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
                default:
                    view.printMsg("Nothing chosen to do");
                    return false;
            }
            return true;
        }

        public void showTable()
        {
            dtManager.Querier.setTable(chosenTable);
            var table = dtManager.Querier.Select();

            string tableStr = $"Table '{chosenTable}'\n";
            table.ForEach(row =>
            {
                tableStr += string.Join(" | ", row) + "\n";
            });
            view.printMsg(tableStr);
        }

        public bool addRow()
        {
            dtManager.Querier.setTable(chosenTable);
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
                dtManager.Querier.Insert(row);
                return true;
            }
            return false;
        }

        public bool chooseTable()
        {
            var tabnames = dtManager.Scaner.getTablesNames();
            if (tabnames.Count == 0)
            {
                view.printMsg("No tables, create one");
                return false;
            }

            var input = view.getChoice(tabnames.ToArray(), "Choose table to interact");
            if (input != null && tabnames.Contains(input))
            {
                chosenTable = input;
                return true;
            }
            return false;
        }
    }
}