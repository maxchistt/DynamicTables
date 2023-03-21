using DynamicTableService;

namespace ConsoleDTApp
{
    internal partial class DTAppController
    {
        private DynamicTableManager dtManager = new();
        private string chosenTable = "";
        private ConsoleView view = new();

        public DTAppController()
        {
            bool resume = true;
            while (resume)
            {
                resume = chooseToDoMain();
            }
        }
    }
}