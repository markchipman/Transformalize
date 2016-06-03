﻿#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using Pipeline.Contracts;
using Quartz;
using Quartz.Spi;

namespace Pipeline.Command {

    public class QuartzJobFactory : IJobFactory {
        private readonly Options _options;
        private readonly IContext _context;
        private readonly ISchemaHelper _schemaHelper;

        public QuartzJobFactory(Options options, IContext context, ISchemaHelper schemaHelper) {
            _options = options;
            _context = context;
            _schemaHelper = schemaHelper;
        }

        public IJob NewJob(TriggerFiredBundle bundle, Quartz.IScheduler scheduler) {
            _context.Debug(() => "Scheduler creating new RunTimeExecutor.");
            return new RunTimeExecutor(_options, _context, _schemaHelper);
        }

        public void ReturnJob(IJob job) {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }

}
