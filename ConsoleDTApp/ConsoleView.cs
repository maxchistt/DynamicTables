using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDTApp
{
    internal class ConsoleView
    {
        public void printMsg(string msg)
        {
            Console.WriteLine(msg);
        }

        public T? getChoseEnum<T>(string? question = null) where T : struct, IConvertible
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

        public string getChose(string[] variants, string? question = null)
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

        public string? getStringname(string? question = null)
        {
            if (question != null) Console.WriteLine(question);
            return Console.ReadLine();
        }
    }
}
