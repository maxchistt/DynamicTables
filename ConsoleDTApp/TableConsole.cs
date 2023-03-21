using DynamicTableService;

namespace ConsoleDTApp
{
    internal class TableConsole
    {
        private DynamicTableManager dtManager = new();
        private string chosenTable = "";
        private ConsoleView view = new();

        private enum todo
        { ChoseTable, CreateTable, ShowTablesList };

        public TableConsole()
        {
            while (true)
            {
                var todoRes = view.getChoseEnum<todo>("Chose what to do");
                switch (todoRes)
                {
                    case todo.CreateTable:
                        view.printMsg(createTable() ? "Table creted" : "No table created");
                        break;

                    case todo.ChoseTable:
                        view.printMsg(choseTable() ? $"Chosen table is '{chosenTable}'" : "No table chosen");
                        break;

                    case todo.ShowTablesList:
                        showTablesList();
                        break;

                    default:
                        view.printMsg("Nothing chosen to do");
                        break;
                }
            }
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

        public bool choseTable()
        {
            var tabnames = dtManager.Scaner.getTablesNames();
            if (tabnames.Count == 0)
            {
                view.printMsg("No tables, create one");
                return false;
            }

            var input = view.getChose(tabnames.ToArray(), "Chose table to interact");
            if (input != null && tabnames.Contains(input))
            {
                chosenTable = input;
                return true;
            }
            return false;
        }
    }
}