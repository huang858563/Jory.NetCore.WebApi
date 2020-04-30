using Microsoft.Extensions.Logging;
using System;
using Jory.NetCore.Core.Helpers;

namespace Jory.NetCore.WebApi.Common
{
    /// <summary>
    /// EFCoreLoggerProvider
    /// </summary>
    public class EfCoreLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new EfLogger(categoryName);

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// EFLogger
    /// </summary>
    public class EfLogger : ILogger
    {
        private readonly string _categoryName;
        public EfLogger(string categoryName) => this._categoryName = categoryName;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (_categoryName == "Microsoft.EntityFrameworkCore.Database.Command" && logLevel == LogLevel.Information)
            {
                var logContent = formatter(state, exception);
                NLogHelper.Logger.Info(logContent);
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
