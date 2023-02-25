using Converter_Microservice.Repositories.Interfaces;
using log4net;
using log4net.Config;
using log4net.Repository;
using System.Reflection;

namespace Converter_Microservice.Repositories.Repositories
{

    public class Log4NetRepository : ILog4NetRepository
    {
        private ILoggerRepository logRepository;
        private readonly ILog _logger;

        public Log4NetRepository()
        {
            logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("App.config"));
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        }

        public void Debug(string message)
        {
            _logger?.Debug(message);
        }

        public void Info(string message)
        {
            _logger?.Info(message);
        }

        public void Error(string message, Exception? ex = null)
        {
            _logger?.Error(message, ex?.InnerException);
        }

        public void Fatal(string message)
        {
            _logger?.Fatal(message);
        }

    }
}
