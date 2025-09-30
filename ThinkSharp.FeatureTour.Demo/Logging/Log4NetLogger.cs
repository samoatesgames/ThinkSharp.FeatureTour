// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using log4net;

namespace ThinkSharp.FeatureTouring.Logging
{
    internal class Log4NetLogger : ILogger
    {
        private static readonly ILog theLogger = LogManager.GetLogger("FeatureTour");

        public void Debug(object message) => theLogger.Debug(message);
        public void Debug(object message, Exception exception) => theLogger.Debug(message, exception);
        public void DebugFormat(string format, params object[] args) => theLogger.DebugFormat(format, args);

        public void Error(object message) => theLogger.Error(message);
        public void Error(object message, Exception exception) => theLogger.Error(message, exception);
        public void ErrorFormat(string format, params object[] args) => theLogger.ErrorFormat(format, args);

        public void Info(object message) => theLogger.Info(message);
        public void Info(object message, Exception exception) => theLogger.Info(message, exception);
        public void InfoFormat(string format, params object[] args) => theLogger.InfoFormat(format, args);

        public void Warn(object message) => theLogger.Warn(message);
        public void Warn(object message, Exception exception) => theLogger.Warn(message, exception);
        public void WarnFormat(string format, params object[] args) => theLogger.WarnFormat(format, args);
    }
}
