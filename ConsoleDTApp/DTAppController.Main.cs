namespace ConsoleDTApp
{
    internal partial class DTAppController
    {
        private enum todoMain
        { ShowTablesList, ShowTableHeader, QueryTable, CreateTable, EditTable, AutoFill };

        public bool chooseToDoMain()
        {
            var todoMainRes = view.getChoiceEnum<todoMain>("Choose what to do");
            if (todoMainRes == null) return false;
            switch (todoMainRes)
            {
                case todoMain.CreateTable:
                    view.printMsg(createTable() ? "Table creted" : "No table created");
                    break;

                case todoMain.ShowTableHeader:
                    showTableHeader();
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

                case todoMain.AutoFill:
                    autoFillTable();
                    break;

                default:
                    view.printMsg("Nothing chosen to do");
                    return false;
            }
            return true;
        }

        public void showTableHeader()
        {
            view.printMsg(chooseTable()
                        ? $"Header of '{chosenTable}':\n{string.Join(" | ", dtManager.Scaner.getColsKeysAndTypes(chosenTable))}"
                        : "No table chosen");
        }

        public void showTablesList()
        {
            var tabnames = dtManager.Scaner.getTablesNames();
            view.printMsg(tabnames.Count > 0
                ? $"Here is the list of tables:\n{string.Join(", ", tabnames)}"
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