namespace IntegrationTests
{
    using System;
    using FizzWare.NBuilder.Generators;
    using Loggly.EntLib.TraceListeners;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            var configSource = ConfigurationSourceFactory.Create();
            var logglyLoggingSettings = (LogglyTraceListenerData)LoggingSettings.GetLoggingSettings(configSource).TraceListeners.Get("Loggly Listener");
            string logglyInputKey = logglyLoggingSettings.LogglyInputKey;

            Console.WriteLine("Testing logging for Loggly input key: {0}", logglyInputKey);

            if (Logger.IsLoggingEnabled())
            {
                for (int i = 1; i < GetRandom.PositiveInt(9) + 1; i++)
                {
                    var message = string.Format("Test {0}", i);
                    Console.WriteLine(message);
                    Logger.Write(message);
                }
            }
        }
    }
}
