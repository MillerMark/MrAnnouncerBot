using System;

namespace MrAnnouncerBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            var mrAnnouncerBot = new MrAnnouncerBot();
            mrAnnouncerBot.Run();
            Console.ReadLine();
            // TODO: async, task, etc.
            mrAnnouncerBot.Disconnect();
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
            Environment.Exit(1);
        }
    }
}