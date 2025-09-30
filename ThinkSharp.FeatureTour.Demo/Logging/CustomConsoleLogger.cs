// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace ThinkSharp.FeatureTouring.Logging
{
    internal class CustomConsoleLogger : ILogger
    {
        private void LogToFile(string obj) => Console.WriteLine($@"FEATURETOUR - {obj}");

        public void Debug(object message) => LogToFile($"DEBUG: {message}");
        public void Debug(object message, Exception exception) => LogToFile($"DEBUG: {message}; Exception: {exception}");
        public void DebugFormat(string format, params object[] args) => LogToFile(string.Format("DEBUG: " + format, args));

        public void Error(object message) => LogToFile($"ERROR: {message}");
        public void Error(object message, Exception exception) => LogToFile($"ERROR: {message}; Exception: {exception}");
        public void ErrorFormat(string format, params object[] args) => LogToFile(string.Format("ERROR: " + format, args));

        public void Info(object message) => LogToFile($"INFO: {message}");
        public void Info(object message, Exception exception) => LogToFile($"INFO: {message}; Exception: {exception}");
        public void InfoFormat(string format, params object[] args) => LogToFile(string.Format("INFO: " + format, args));

        public void Warn(object message) => LogToFile($"WARNING: {message}");
        public void Warn(object message, Exception exception) => LogToFile($"WARNING: {message}; Exception: {exception}");
        public void WarnFormat(string format, params object[] args) => LogToFile(string.Format("WARNING: " + format, args));
    }
}
