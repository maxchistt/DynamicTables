﻿namespace ConsoleDTApp
{
    internal class ConsoleView
    {
        public void printMsg(string msg)
        {
            Console.WriteLine(msg);
        }

        public T? getChoiceEnum<T>(string? question = null) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            else
            {
                string choiceRes = getChoice(Enum.GetNames(typeof(T)), question);
                if (choiceRes == null || choiceRes == "") return null;
                return (T)Enum.Parse(typeof(T), choiceRes, true);
            }
        }

        private readonly List<char> keyList = new() { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm' };

        public string getChoice(string[] variants, string? question = null)
        {
            Console.WriteLine();
            if (question != null) Console.WriteLine(question);

            if (variants.Length == 0 || variants.Length > keyList.Count)
            {
                Console.WriteLine(variants.Length > 0
                    ? $"It need be not more {keyList.Count} variants for getChoice(string[] variants, ...)"
                    : "No variants to choose");
                return "";
            }

            string varlist = "Press key to choose variant:\n";
            for (int i = 0; i < variants.Length; i++)
            {
                varlist += $"{keyList[i]}. {variants[i]}\n";
            }
            Console.WriteLine(varlist);

            var input = Console.ReadKey(true);
            if (keyList.Contains(input.KeyChar) && keyList.IndexOf(input.KeyChar) < variants.Length)
                return variants[keyList.IndexOf(input.KeyChar)];
            return "";
        }

        public string? getStringname(string? question = null)
        {
            if (question != null) Console.WriteLine(question);
            return Console.ReadLine();
        }
    }
}