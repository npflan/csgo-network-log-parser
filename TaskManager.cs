using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSGO_DataLogger
{
    internal class TaskManager
    {
        private Task task;
        protected CancellationTokenSource cancellationTokenSource;
        protected CancellationToken cancelationToken;
        private int _port;

        public void Start(int port, bool overideShutdown = false)
        {
            _port = port;
            WriteLine("Task start is requestet");

            if (IsRunning())
            {
                WriteLine("Task is allready running");
                return;
            }


            cancellationTokenSource = new CancellationTokenSource();
            cancelationToken = cancellationTokenSource.Token;
            task = new Task(() =>
            {
                TaskWork();
            });
            task.Start();

            WriteLine("Task is startet");

        }

        public bool IsRunning() => (task != null && task.Status == TaskStatus.Running);

        public void Stop(bool shutdown = false)
        {
            WriteLine("Task stop is requestet");

            if (!IsRunning())
            {
                WriteLine("Task is not running");
                return;
            }

            cancellationTokenSource.Cancel();
            WriteLine("Task cancellation sendt");
        }

        private void TaskWork()
        {
            Task.Run(async () =>
            {
                using (var udpClient = new UdpClient(_port))
                {
                    long counter = 0;
                    while (true)
                    {
                        Console.Write($"\rAvalible:{udpClient.Available} Counter:{counter}                      ");
                        //if (udpClient.Available > 0)
                        //WriteLine($"Socket data avalible {udpClient.Available}");
                        var receivedResults = await udpClient.ReceiveAsync();
                        counter++;
                        Task.Run(() => LogManager.ParseLog(receivedResults.RemoteEndPoint.Address.ToString(), Encoding.ASCII.GetString(receivedResults.Buffer)));

                        //ClassLibrary_ParseMessage.Manager.ParseLog(receivedResults.RemoteEndPoint.Address.ToString(), Encoding.ASCII.GetString(receivedResults.Buffer));
                    }
                }
            }, cancelationToken);

            //Keep the task alive and kill it the same time as the async
            cancelationToken.WaitHandle.WaitOne();
        }

        private void WriteLine(string text)
        {
            Task.Run(() =>
            {
                Console.WriteLine(text);
            });
        }
    }
}