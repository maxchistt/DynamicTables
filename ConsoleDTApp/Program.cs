using DynamicTableService;
using Newtonsoft.Json;

namespace ConsoleDTApp
{
    internal class Program
    {
        static DTAppController consoleApp;
        private static void Main(string[] args)
        {
            Console.WriteLine("DTServiceTestApp");
            consoleApp = new DTAppController();
        }
    }
}