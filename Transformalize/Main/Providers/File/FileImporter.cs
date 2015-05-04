﻿using System.Globalization;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {

    public class FileImporter {
        private readonly ILogger _logger;

        public FileImporter(ILogger logger)
        {
            _logger = logger;
        }

        public long ImportScaler(string fileName, TflConnection output) {
            return ImportScaler(new FileInspectionRequest(fileName), output);
        }

        public long ImportScaler(FileInspectionRequest request, TflConnection output) {
            var fileInformation = FileInformationFactory.Create(request, _logger);
            var configuration = BuildProcess(fileInformation, request, output, _logger);
            var initProcess = ProcessFactory.CreateSingle(configuration, _logger);
            if (initProcess.Connections.Output().Provider != "internal") {
                initProcess.Options.Mode = "init";
                initProcess.Mode = "init";
                initProcess.ExecuteScaler();
            }

            var process = ProcessFactory.CreateSingle(configuration, _logger, new Options());
            process.ExecuteScaler();
            return process.Entities[0].Inserts;
        }

        public FileImportResult Import(string fileName, TflConnection output) {
            return Import(new FileInspectionRequest(fileName), output);
        }

        public FileImportResult Import(FileInspectionRequest request, TflConnection output) {

            var fileInformation = FileInformationFactory.Create(request, _logger);
            var configuration = BuildProcess(fileInformation, request, output, _logger);
            var process = ProcessFactory.CreateSingle(configuration, _logger);

            if (process.Connections.Output().Provider != "internal") {
                process.Options.Mode = "init";
                process.Mode = "init";
                process.ExecuteScaler();
            }

            return new FileImportResult {
                Information = fileInformation,
                Rows = ProcessFactory.CreateSingle(configuration, _logger).Execute()
            };
        }

        private static TflProcess BuildProcess(FileInformation fileInformation, FileInspectionRequest request, TflConnection output, ILogger logger) {

            var root = new TflRoot(string.Format(@"<tfl><processes><add name='{0}'><connections><add name='input' provider='internal' /></connections></add></processes></tfl>", request.EntityName), null);

            var process = root.GetDefaultOf<TflProcess>(p => {
                p.Name = request.EntityName;
                p.Star = request.ProcessName;
                p.StarEnabled = false;
            });

            process.Connections.Add(process.GetDefaultOf<TflConnection>(c => {
                c.Name = "input";
                c.Provider = "file";
                c.File = fileInformation.FileInfo.FullName;
                c.Delimiter = fileInformation.Delimiter == default(char) ? "|" : fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture);
                c.Start = fileInformation.FirstRowIsHeader ? 2 : 1;
            }));

            process.Connections.Add(output);

            process.Entities.Add(process.GetDefaultOf<TflEntity>(e => {
                e.Name = request.EntityName;
                e.PrependProcessNameToOutputName = false;
                e.DetectChanges = false;
            }));

            var fields = new FieldInspector(logger).Inspect(fileInformation, request);

            foreach (var fd in fields) {
                if (fd.Type.Equals("string")) {
                    logger.EntityInfo(request.EntityName, "Using {0} character string for {1}.", fd.Length, fd.Name);
                } else {
                    logger.EntityInfo(request.EntityName, "Using {0} for {1}.", fd.Type, fd.Name);
                }

                var field = fd;
                process.Entities[0].Fields.Add(process.GetDefaultOf<TflField>(f => {
                    f.Name = field.Name;
                    f.Length = field.Length;
                    f.Type = field.Type;
                    f.QuotedWith = field.QuotedWith;
                }));
            }

            return process;
        }

    }
}