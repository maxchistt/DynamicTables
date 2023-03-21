using DynamicTableService;
using Newtonsoft.Json;

namespace ConsoleDTApp
{
    internal class Program
    {
        static TableConsole consoleApp;
        private static void Main(string[] args)
        {
            Console.WriteLine("DTServiceTestApp");
            consoleApp = new TableConsole();
        }
    }
}