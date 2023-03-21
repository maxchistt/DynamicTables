using DynamicTableService;

namespace ConsoleDTApp
{
    internal class TableConsole
    {
        private DynamicTableManager dtManager = new();
        private string chosenTable = "";
        private ConsoleView view = new();

        private enum todoMain
        { ChooseTable, CreateTable, EditTable, ShowTablesList };

        private enum todoEditTable
        { RenameTable, AddColumn, EditColumn, RenameColumn, AddPrimaryKey, AddPriamryKeys, DropTable, DropColumn, DropPrimaryKeys };

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
                    view.printMsg(chooseTable() ? $"Chosen table is '{chosenTable}'" : "No table chosen");
                    break;

                case todoMain.ShowTablesList:
                    showTablesList();
                    break;

                case todoMain.EditTable:
                    editTable();
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
                ? $"Here is the list of tables:\n {string.Join(",", tabnames)}"
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