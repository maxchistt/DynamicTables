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
        private DynamicTableManager dtManager = new();
        private string chosenTable = "";

        public TableConsole()
        {
            bool res = choseTable();
            if (res)
            {
                Console.WriteLine($"Chosen table is: '{chosenTable}'");
            }
            else
            {
                Console.WriteLine("No chosen table");
            }
        }

        public bool choseTable()
        {
            var tabnames = dtManager.Scaner.getTablesNames();

            if(tabnames.Count == 0)
            {
                Console.WriteLine("No tables, create one");
                return false;
            }

            var input = getChose(tabnames.ToArray(), "Chose table to interact");

            if (input != null && tabnames.Contains(input))
            {
                chosenTable = input;
                return true;
            }
            return false;
        }

        private string getChose(string[] variants, string? question)
        {
            if (question != null) Console.WriteLine(question);
            if (variants.Length == 0)
            {
                Console.WriteLine("No variants to chose");
                return "";
            }
            string varlist = "Press key to chose variant:\n";
            for (int i = 0; i < variants.Length; i++)
            {
                varlist += $"{i}. {variants[i]}";
            }
            var input = Console.ReadKey(true);
            int Num;
            if (
             int.TryParse(input.KeyChar.ToString(), out Num)
             && Num < variants.Length && Num >= 0
            )
            {
                return variants[Num];
            }
            return "";
        }
    }
}