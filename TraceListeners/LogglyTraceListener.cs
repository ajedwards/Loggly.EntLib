namespace Loggly.EntLib.TraceListeners
{
    using System.Diagnostics;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
    using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

    [ConfigurationElementType(typeof(LogglyTraceListenerData))]
    public sealed class LogglyTraceListener : CustomTraceListener
    {
        public LogglyTraceListener(string name, ILogFormatter formatter, string logglyInputKey)
        {
            if (string.IsNullOrEmpty(logglyInputKey))
            {
                throw new ConfigurationSourceErrorsException("Invalid values in LogglyTraceListener configuration.");
            }
            else
            {
                this.Logger = new Loggly.Logger(logglyInputKey);
            }

            this.Formatter = formatter;
            this.InputKey = logglyInputKey;
            this.Name = name;
        }

        public string InputKey { get; set; }

        public ILogger Logger { get; set; }

        public override void TraceData(
            TraceEventCache eventCache, 
            string source, 
            TraceEventType eventType, 
            int id, 
            object data)
        {
            if (this.Filter == null || 
                this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                if (data is LogEntry && this.Formatter != null)
                {
                    this.WriteLine(this.Formatter.Format(data as LogEntry));
                }
                else if (data != null)
                {
                    this.WriteLine(data.ToString());
                }
                else
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                }
            }
        }

        public override void Write(string message)
        {
            this.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            if (this.Logger != null)
            {
                this.Logger.Log(message);
            }
        }
    }
}
