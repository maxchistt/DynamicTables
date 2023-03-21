using DynamicTableService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDTApp
{
    internal class TableConsole
    {
        private DynamicTableManager dtManager = new();
        private string chosenTable = "";

        private enum todo { ChoseTable, CreateTable, ShowTablesList };

        public TableConsole()
        {
            while (true)
            {
                var todoRes = getChoseEnum<todo>("Chose what to do");
                switch (todoRes)
                {
                    case todo.CreateTable:
                        Console.WriteLine(createTable() ? "Table creted" : "No table created");
                        break;
                    case todo.ChoseTable:
                        Console.WriteLine(choseTable() ? $"Chosen table is '{chosenTable}'" : "No table chosen");
                        break;
                    case todo.ShowTablesList:
                        showTablesList();
                        break;
                    default:
                        Console.WriteLine("Nothing chosen to do");
                        break;
                }
            }
        }

        public void showTablesList()
        {
            var tabnames = dtManager.Scaner.getTablesNames();
            Console.WriteLine(tabnames.Count > 0
                ? $"Here is the list of tables:\n {string.Join(",", tabnames)}"
                : "No tables in Db");
        }

        public bool createTable()
        {
            var tabName = getStringname("Enter name of table to create:");
            if (tabName == null || tabName == "") return false;
            dtManager.Builder.CreateTable(tabName);
            return true;
        }

        public bool choseTable()
        {
            var tabnames = dtManager.Scaner.getTablesNames();

            if (tabnames.Count == 0)
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



        private T? getChoseEnum<T>(string? question = null) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
                return null;
            }
            else
            {
                List<string> variantNames = new();
                foreach (string variant in Enum.GetNames(typeof(T)))
                {
                    variantNames.Add(variant);
                }
                string choseRes = getChose(variantNames.ToArray(), question);

                if (choseRes == null || choseRes == "") return null;

                T enumRes = (T)Enum.Parse(typeof(T), choseRes, true);

                return enumRes;
            }
            return null;
        }

        private string getChose(string[] variants, string? question = null)
        {
            Console.WriteLine();
            if (variants.Length > 10)
            {
                Console.WriteLine("It need be not more 10 variants for getChose(string[] variants, ...)");
                return "";
            }

            if (question != null) Console.WriteLine(question);

            if (variants.Length == 0)
            {
                Console.WriteLine("No variants to chose");
                return "";
            }
            string varlist = "Press key to chose variant:\n";
            for (int i = 0; i < variants.Length; i++)
            {
                varlist += $"{i + 1}. {variants[i]}\n";
            }
            Console.WriteLine(varlist);

            var input = Console.ReadKey(true);
            int Num;
            if (int.TryParse(input.KeyChar.ToString(), out Num))
            {
                if (Num == 0) Num = 10;
                Num--;

                if (Num < variants.Length && Num >= 0) return variants[Num];
            }
            return "";
        }

        private string? getStringname(string? question = null)
        {
            if (question != null) Console.WriteLine(question);
            return Console.ReadLine();
        }
    }
}