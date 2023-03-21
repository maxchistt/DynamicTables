namespace ConsoleDTApp
{
    internal class Program
    {
        private static DTAppController consoleApp;

        private static void Main(string[] args)
        {
            Console.WriteLine("DTServiceTestApp");
            consoleApp = new DTAppController();
        }
    }
}