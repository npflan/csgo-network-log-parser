using System;

namespace CSGO_DataLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to test console");

            WebCallManager.MakeWebCall("http://localhost:5000/webhook?action=bomb&token=kdjfngjidfsnglkjdfsngiusngu");

            ListenManager.Start();

            Console.WriteLine("Press any key to stop system");
            Console.ReadKey();

            ListenManager.Stop();

            Console.WriteLine("Press any key to exit console");
            Console.ReadKey();
        }
    }
}
