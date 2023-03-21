using DynamicTableService;

namespace ConsoleDTApp
{
    internal class TableConsole
    {
        private DynamicTableManager dtManager = new();
        private string chosenTable = "";
        private ConsoleView view = new();

        private enum todo
        { ChooseTable, CreateTable, ShowTablesList };

        public TableConsole()
        {
            while (true)
            {
                chooseToDoMain();
            }
        }

        public bool chooseToDoMain()
        {
            var todoRes = view.getChoiceEnum<todo>("Chose what to do");
            if (todoRes == null) return false;
            switch (todoRes)
            {
                case todo.CreateTable:
                    view.printMsg(createTable() ? "Table creted" : "No table created");
                    break;

                case todo.ChooseTable:
                    view.printMsg(chooseTable() ? $"Chosen table is '{chosenTable}'" : "No table chosen");
                    break;

                case todo.ShowTablesList:
                    showTablesList();
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