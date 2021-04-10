using Microsoft.SqlServer.XEvent.XELite;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace XELiveSteamConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            

            OutputXELStream(AppConfig.GetConnectionStringByName("mydb"), AppConfig.GetAppConfig("sessionName"));
        }

        static void OutputXELStream(string connectionString, string sessionName)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var xeStream = new XELiveEventStreamer(connectionString, sessionName);

            Console.WriteLine("Press any key to stop listening...");
            Task waitTask = Task.Run(() =>
            {
                Console.ReadKey();
                cancellationTokenSource.Cancel();
            });

            Task readTask = xeStream.ReadEventStream(() =>
            {
                Console.WriteLine("Connected to session");
                return Task.CompletedTask;
            },
                xevent =>
                {
                    Console.WriteLine(xevent);
                    Console.WriteLine("");
                    return Task.CompletedTask;
                },
                cancellationTokenSource.Token);


            try
            {
                Task.WaitAny(waitTask, readTask);
            } catch (TaskCanceledException)
            {
            }

            if (readTask.IsFaulted)
            {
                Console.Error.WriteLine("Failed with: {0}", readTask.Exception);
            }
        }
    }
}
