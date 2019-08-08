using System;
using System.Threading;
using System.Threading.Tasks;
using Senko.Framework.Hosting;

namespace Senko.Framework
{
    public static class BotHostExtensions
    {
        public static async Task RunAsync(this IBotHost host)
        {
            var done = new ManualResetEventSlim(false);

            try
            {
                using var cts = new CancellationTokenSource();

                AttachCtrlcSigtermShutdown(cts, done, "Bot is shutting down...");

                await host.StartAsync(cts.Token);
                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (TaskCanceledException)
            {
                // ignore.
            }
            finally
            {
                done.Set();
            }

            await host.StopAsync();
        }

        private static void AttachCtrlcSigtermShutdown(CancellationTokenSource cts, ManualResetEventSlim resetEvent, string shutdownMessage)
        {
            void Shutdown()
            {
                if (!cts.IsCancellationRequested)
                {
                    if (!string.IsNullOrEmpty(shutdownMessage))
                    {
                        Console.WriteLine(shutdownMessage);
                    }
                    try
                    {
                        cts.Cancel();
                    }
                    catch (ObjectDisposedException) { }
                }

                // Wait on the given reset event
                resetEvent.Wait();
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Shutdown();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Shutdown();
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
        }
    }
}
