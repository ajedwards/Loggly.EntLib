namespace Loggly.EntLib.TraceListeners
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

    public class LogglyTraceListenerData : TraceListenerData
    {
        private const string FormatterProperty = "formatter";
        private const string LogglyInputKeyProperty = "logglyInputKey";

        [ConfigurationProperty(LogglyTraceListenerData.FormatterProperty)]
        [Reference(typeof(NameTypeConfigurationElementCollection<FormatterData, CustomFormatterData>), typeof(FormatterData))]
        public string Formatter
        {
            get
            {
                return (string)base[LogglyTraceListenerData.FormatterProperty];
            }
            set
            {
                base[LogglyTraceListenerData.FormatterProperty] = value;
            }
        }

        [ConfigurationProperty(LogglyTraceListenerData.LogglyInputKeyProperty, IsRequired = true)]
        public string LogglyInputKey
        {
            get
            {
                return (string)base[LogglyTraceListenerData.LogglyInputKeyProperty];
            }
            set
            {
                base[LogglyTraceListenerData.LogglyInputKeyProperty] = value;
            }
        }

        protected override Expression<Func<TraceListener>> GetCreationExpression()
        {
            return () =>
                new LogglyTraceListener(
                    this.Name, 
                    Container.ResolvedIfNotNull<ILogFormatter>(this.Formatter), 
                    this.LogglyInputKey);
        }
    }
}
