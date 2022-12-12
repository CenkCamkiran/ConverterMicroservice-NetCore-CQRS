using log4net;
using log4net.Config;
using log4net.Repository;
using System.Reflection;

namespace DataAccess
{
    public interface ILogger
    {
        void Debug(string message);

        void Fatal(string message);

        void Info(string message);

        void Error(string message, Exception? ex = null);
    }

    public class Logger : ILogger
    {
        private ILoggerRepository logRepository;
        private readonly ILog _logger;

        public Logger()
        {
            logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("App.config"));
            this._logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        }

        public void Debug(string message)
        {
            this._logger?.Debug(message);
        }

        public void Info(string message)
        {
            this._logger?.Info(message);
        }

        public void Error(string message, Exception? ex = null)
        {
            this._logger?.Error(message, ex?.InnerException);
        }

        public void Fatal(string message)
        {
            this._logger?.Fatal(message);
        }

    }
}
