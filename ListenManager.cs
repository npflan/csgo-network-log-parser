using System;
using System.Threading.Tasks;

namespace CSGO_DataLogger
{
    public static class ListenManager
    {
        private static TaskManager taskManager = new TaskManager();

        public static void Start(int port = 9998)
        {
            Console.WriteLine("UDPListen - Manger - Start is requestet");
            taskManager.Start(port);
        }

        public static void Stop(bool WaitForExit = true)
        {
            Console.WriteLine("UDPListen - Manger - Stop is requestet");
            taskManager.Stop();

            Task shutdownTask = new Task(() =>
            {
                while (1 == 1)
                {
                    bool _taskManagerIsRunning = taskManagerIsRunning();

                    if (
                        !_taskManagerIsRunning
                        )
                        break;

                    System.Threading.Thread.Sleep(1000);
                }
                Console.WriteLine("UDPListen - Manger - Stop is completed");
            });

            if (WaitForExit)
            {
                shutdownTask.RunSynchronously();
            }
            else
            {
                shutdownTask.Start();
            }
        }

        public static bool taskManagerIsRunning() => taskManager.IsRunning();
    }
}