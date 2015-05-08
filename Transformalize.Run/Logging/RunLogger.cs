﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Transformalize.Configuration;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Run.Libs.SemanticLogging;
using Transformalize.Run.Libs.SemanticLogging.TextFile;
using Transformalize.Run.Libs.SemanticLogging.TextFile.Sinks;

namespace Transformalize.Run.Logging {

    public class RunLogger : ILogger {

        public string Name { get; set; }
        private List<ObservableEventListener> _eventListeners = new List<ObservableEventListener>();
        private List<SinkSubscription> _sinkSubscriptions = new List<SinkSubscription>();
        private bool _started;

        public List<ObservableEventListener> EventListeners {
            get { return _eventListeners; }
            set { _eventListeners = value; }
        }

        public List<SinkSubscription> SinkSubscriptions {
            get { return _sinkSubscriptions; }
            set { _sinkSubscriptions = value; }
        }

        public void Info(string message, params object[] args) {
            EntityInfo(".", message, args);
        }

        public void Debug(string message, params object[] args) {
            EntityDebug(".", message, args);
        }

        public void Warn(string message, params object[] args) {
            EntityWarn(".", message, args);
        }

        public void Error(string message, params object[] args) {
            EntityError(".", message, args);
        }

        public void Error(Exception exception, string message, params object[] args) {
            EntityError(".", exception, message, args);
        }

        public void EntityInfo(string entity, string message, params object[] args) {
            TflLogger.Info(Name, entity, message, args);
        }

        public void EntityDebug(string entity, string message, params object[] args) {
            TflLogger.Debug(Name, entity, message, args);
        }

        public void EntityWarn(string entity, string message, params object[] args) {
            TflLogger.Warn(Name, entity, message, args);
        }

        public void EntityError(string entity, string message, params object[] args) {
            TflLogger.Error(Name, entity, message, args);
        }

        public void EntityError(string entity, Exception exception, string message, params object[] args) {
            TflLogger.Error(Name, entity, message, args);
            TflLogger.Error(Name, entity, exception.Message);
            TflLogger.Error(Name, entity, exception.StackTrace);
        }

        public void Start(TflProcess process) {
            if (_started)
                return;

            _started = true;
            Name = process.Name;
            foreach (var log in process.Log) {
                switch (log.Provider) {
                    case "file":
                        log.Folder = log.Folder.Replace('/', '\\');
                        log.File = log.File.Replace('/', '\\');
                        log.Folder = (log.Folder.Equals(Common.DefaultValue) ? "logs" : log.Folder).TrimEnd('\\') + "\\";
                        log.File = (log.File.Equals(Common.DefaultValue) ? "tfl-" + process.Name + ".log" : log.File).TrimStart('\\');

                        var fileListener = new ObservableEventListener();
                        fileListener.EnableEvents(TflEventSource.Log, (EventLevel)Enum.Parse(typeof(EventLevel), log.Level));
                        SinkSubscriptions.Add(fileListener.LogToRollingFlatFile(log.Folder + log.File, 5000, "yyyy-MM-dd", RollFileExistsBehavior.Increment, RollInterval.Day, new LegacyLogFormatter(), 0, log.Async));
                        EventListeners.Add(fileListener);
                        break;
                    case "mail":
                        if (log.Subject.Equals(Common.DefaultValue)) {
                            log.Subject = process.Name + " " + log.Level;
                        }
                        var mailListener = new ObservableEventListener();
                        mailListener.EnableEvents(TflEventSource.Log, EventLevel.Error);
                        SinkSubscriptions.Add(mailListener.LogToEmail(log));
                        EventListeners.Add(mailListener);
                        break;
                }

            }

        }

        public void Stop() {
            foreach (var listener in EventListeners) {
                listener.Dispose();
            }
            foreach (var sink in SinkSubscriptions) {
                sink.Dispose();
            }
        }
    }
}