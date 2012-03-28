namespace Loggly.EntLib.UnitTests
{
    using System;
    using System.Diagnostics;
    using FizzWare.NBuilder;
    using FizzWare.NBuilder.Generators;
    using FluentAssertions;
    using Loggly.EntLib.TraceListeners;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class LogglyTraceListenerTester : IDisposable
    {
        private bool _isDisposed;
        private ILogFormatter _mockLogFormatter;
        private ILogger _mockLogger;
        private LogglyTraceListener _targetListener;

        [TestCleanup]
        public void CleanupTest()
        {
            this._mockLogFormatter = null;
            this._mockLogger = null;

            if (this._targetListener != null)
            {
                this._targetListener.Dispose();
                this._targetListener = null;
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
            this._mockLogFormatter = Substitute.For<ILogFormatter>();
            this._mockLogger = Substitute.For<ILogger>();
            this._targetListener = new LogglyTraceListener(GetRandom.String(10), this._mockLogFormatter, this._mockLogger);
        }

        [TestMethod]
        public void TestConstructorSetsExpectedFormatter()
        {
            var expectedFormatter = Substitute.For<ILogFormatter>();

            using (var newListener = new LogglyTraceListener(GetRandom.String(10), expectedFormatter, this._mockLogger))
            {
                newListener.Formatter.Should().BeSameAs(expectedFormatter);
            }
        }

        [TestMethod]
        public void TestConstructorSetsExpectedLogger()
        {
            var expectedLogger = Substitute.For<ILogger>();

            using (var newListener = new LogglyTraceListener(GetRandom.String(10), this._mockLogFormatter, expectedLogger))
            {
                newListener.Logger.Should().BeSameAs(expectedLogger);
            }
        }

        [TestMethod]
        public void TestConstructorSetsExpectedName()
        {
            var expectedName = GetRandom.String(10);

            using (var newListener = new LogglyTraceListener(expectedName, this._mockLogFormatter, this._mockLogger))
            {
                newListener.Name.Should().Be(expectedName);
            }
        }

        [TestMethod]
        public void TestTraceDataDoesNotLogToLogglyWhenEventTypeDoesNotMatchTraceFilter()
        {
            this._targetListener.Filter = new EventTypeFilter(SourceLevels.Off);
            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, null);

            this._mockLogger.DidNotReceive().Log(Arg.Any<string>());
        }

        [TestMethod]
        public void TestTraceDataDoesNotLogToLogglyWhenLoggerIsNull()
        {
            this._targetListener.Logger = null;
            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, null);

            this._mockLogger.DidNotReceive().Log(Arg.Any<string>());
        }

        [TestMethod]
        public void TestTraceDataDoesNotLogToLogglyWhenSourceDoesNotMatchTraceFilter()
        {
            var expectedSource = GetRandom.String(10);

            this._targetListener.Filter = new SourceFilter(string.Empty);
            this._targetListener.TraceData(null, expectedSource, GetRandom.Enumeration<TraceEventType>(), 0, null);

            this._mockLogger.DidNotReceive().Log(Arg.Any<string>());
        }

        [TestMethod]
        public void TestTraceDataLogsExpectedFormattedLogEntryToLogglyWhenEventTypeMatchesTraceFilter()
        {
            var expectedFormattedLogEntry = GetRandom.String(10);

            var expectedLogEntry = Builder<LogEntry>.CreateNew().Build();
            this._mockLogFormatter.Format(expectedLogEntry).Returns(expectedFormattedLogEntry);

            this._targetListener.Filter = new EventTypeFilter(SourceLevels.All);
            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, expectedLogEntry);

            this._mockLogger.Received().Log(expectedFormattedLogEntry);
        }

        [TestMethod]
        public void TestTraceDataLogsExpectedFormattedLogEntryToLogglyWhenSourceMatchesTraceFilter()
        {
            var expectedFormattedLogEntry = GetRandom.String(10);
            var expectedSource = GetRandom.String(10);

            var expectedLogEntry = Builder<LogEntry>.CreateNew().Build();
            this._mockLogFormatter.Format(expectedLogEntry).Returns(expectedFormattedLogEntry);

            this._targetListener.Filter = new SourceFilter(expectedSource);
            this._targetListener.TraceData(null, expectedSource, GetRandom.Enumeration<TraceEventType>(), 0, expectedLogEntry);

            this._mockLogger.Received().Log(expectedFormattedLogEntry);
        }

        [TestMethod]
        public void TestTraceDataLogsExpectedFormattedLogEntryToLogglyWhenTraceFilterIsNull()
        {
            var expectedFormattedLogEntry = GetRandom.String(10);

            var expectedLogEntry = Builder<LogEntry>.CreateNew().Build();
            this._mockLogFormatter.Format(expectedLogEntry).Returns(expectedFormattedLogEntry);

            this._targetListener.Filter = null;
            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, expectedLogEntry);

            this._mockLogger.Received().Log(expectedFormattedLogEntry);
        }

        [TestMethod]
        public void TestTraceDataLogsExpectedMessageToLogglyForNonLogEntryDataWhenEventTypeMatchesTraceFilter()
        {
            var expectedData = new object();
            var expectedMessage = expectedData.ToString();

            this._targetListener.Filter = new EventTypeFilter(SourceLevels.All);
            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, expectedData);

            this._mockLogger.Received().Log(expectedMessage);
        }

        [TestMethod]
        public void TestTraceDataLogsExpectedMessageToLogglyForNonLogEntryDataWhenFormatterIsNull()
        {
            var expectedData = new object();
            var expectedMessage = expectedData.ToString();

            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, expectedData);

            this._mockLogger.Received().Log(expectedMessage);
        }

        [TestMethod]
        public void TestTraceDataLogsExpectedMessageToLogglyForNonLogEntryDataWhenTraceFilterIsNull()
        {
            var expectedData = new object();
            var expectedMessage = expectedData.ToString();

            this._targetListener.Filter = null;
            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, expectedData);

            this._mockLogger.Received().Log(expectedMessage);
        }

        [TestMethod]
        public void TestTraceDataLogsEmptyMessageToLogglyForNullDataWhenEventTypeMatchesTraceFilter()
        {
            this._targetListener.Filter = new EventTypeFilter(SourceLevels.All);
            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, (object)null);

            this._mockLogger.Received().Log(string.Empty);
        }

        [TestMethod]
        public void TestTraceDataLogsEmptyMessageToLogglyForNullDataWhenSourceMatchesTraceFilter()
        {
            var expectedSource = GetRandom.String(10);

            this._targetListener.Filter = new SourceFilter(expectedSource);
            this._targetListener.TraceData(null, expectedSource, GetRandom.Enumeration<TraceEventType>(), 0, (object)null);

            this._mockLogger.Received().Log(string.Empty);
        }

        [TestMethod]
        public void TestTraceDataLogsEmptyMessageToLogglyForNullDataWhenTraceFilterIsNull()
        {
            this._targetListener.Filter = null;
            this._targetListener.TraceData(null, null, GetRandom.Enumeration<TraceEventType>(), 0, (object)null);

            this._mockLogger.Received().Log(string.Empty);
        }

        [TestMethod]
        public void TestWriteLogsExpectedMessageToLoggly()
        {
            var expectedMessage = GetRandom.String(10);

            this._targetListener.Write(expectedMessage);

            this._mockLogger.Received().Log(expectedMessage);
        }

        [TestMethod]
        public void TestWriteLineLogsExpectedMessageToLoggly()
        {
            var expectedMessage = GetRandom.String(10);

            this._targetListener.WriteLine(expectedMessage);

            this._mockLogger.Received().Log(expectedMessage);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (!this._isDisposed && this._targetListener != null)
                {
                    this._targetListener.Dispose();
                    this._targetListener = null;
                }
            }

            this._isDisposed = true;
        }
    }
}
