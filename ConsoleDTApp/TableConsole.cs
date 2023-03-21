using DynamicTableService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDTApp
{
    internal class TableConsole
    {
        DynamicTableManager dtManager = new();
        string chosenTable = "";
        
        public TableConsole() {
            bool res = choseTable();
            if (res)
            {
                
            }
        }

        public bool choseTable()
        {
            var tabnames = dtManager.Scaner.getTablesNames();

            var tabsStr = JsonConvert.SerializeObject(tabnames);

            Console.WriteLine($"Here is the list of tables: {tabsStr}. \nChose name of table you need:");
            var input = Console.ReadLine();

            if (input != null && tabnames.Contains(input))
            {
                chosenTable = input;
                return true;
            }
            return false;
        }
    }
}
