using System;
using Transformalize.Configuration;

namespace Transformalize.Logging {
    public interface ILogger {

        string Name { get; set; }

        void Info(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(string message, params object[] args);
        void Error(Exception exception, string message, params object[] args);

        void EntityInfo(string entity, string message, params object[] args);
        void EntityDebug(string entity, string message, params object[] args);
        void EntityWarn(string entity, string message, params object[] args);
        void EntityError(string entity, string message, params object[] args);
        void EntityError(string entity, Exception exception, string message, params object[] args);

        void Start(TflProcess process);
        void Stop();
    }
}