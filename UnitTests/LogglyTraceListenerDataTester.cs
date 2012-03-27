namespace Loggly.EntLib.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Loggly.EntLib.TraceListeners;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LogglyTraceListenerDataTester : IDisposable
    {
        private IUnityContainer _container;
        private bool _isDisposed;

        [TestCleanup]
        public void CleanupTest()
        {
            if (this._container != null)
            {
                this._container.Dispose();
                this._container = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [TestInitialize]
        public void InitializeTest()
        {
            this._container = new UnityContainer()
                .AddNewExtension<EnterpriseLibraryCoreExtension>();
        }

        ////[TestMethod]
        ////public void TestConstructorSetsExpectedLogglyInputKey()
        ////{
        ////    string expectedInputKey = GetRandom.String(10);

        ////    var newListener = new LogglyTraceListenerData(GetRandom.String(10), expectedInputKey);

        ////    newListener.LogglyInputKey.Should().Be(expectedInputKey);
        ////}

        [TestMethod]
        public void TestConstructorSetsDefaultListenerDataType()
        {
            var newListener = new LogglyTraceListenerData();

            newListener.ListenerDataType.Should().Be(typeof(LogglyTraceListenerData));
        }

        [TestMethod]
        public void TestConstructorSetsDefaultName()
        {
            var newListener = new LogglyTraceListenerData();

            newListener.Name.Should().Be("unnamed");
        }

        ////[TestMethod]
        ////public void TestConstructorSetsExpectedName()
        ////{
        ////    var expectedName = GetRandom.String(10);

        ////    var newListener = new LogglyTraceListenerData(expectedName);

        ////    newListener.Name.Should().Be(expectedName);
        ////}

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (!this._isDisposed && this._container != null)
                {
                    this._container.Dispose();
                    this._container = null;
                }
            }

            this._isDisposed = true;
        }

        private static LogglyTraceListenerData GetLogglyTraceListenerSettings()
        {
            LogglyTraceListenerData traceListenerSettings = null;

            using (var configurationSource = ConfigurationSourceFactory.Create())
            {
                var loggingSettings = LoggingSettings.GetLoggingSettings(configurationSource);
                traceListenerSettings = loggingSettings.TraceListeners
                    .Where(l => l is LogglyTraceListenerData)
                    .Single() as LogglyTraceListenerData;
            }

            return traceListenerSettings;
        }
    }
}
