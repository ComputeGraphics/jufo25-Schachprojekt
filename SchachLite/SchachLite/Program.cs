// See https://aka.ms/new-console-template for more information

namespace SchachLite
{
    internal class Init
    {
        public static void Main()
        {
            Storage.start();
            MainMenu();
        }

        public static void MainMenu()
        {
            while(!Console.KeyAvailable) { }
            Console.WriteLine("Start...");
            processor.show();
        }
    }
}