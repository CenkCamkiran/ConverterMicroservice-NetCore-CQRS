﻿using DataLayer.Interfaces;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataAccess
{
    public class Log4NetRepository: ILog4NetRepository
    {
        private ILoggerRepository logRepository;
        private readonly ILog _logger;

        public Log4NetRepository()
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
